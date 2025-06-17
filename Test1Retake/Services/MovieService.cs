using Microsoft.Data.SqlClient;
using Test1Retake.Contracts.Requests;
using Test1Retake.Contracts.Responses;
using Test1Retake.Exceptions;
using Test1Retake.Infrastructure;
using Test1Retake.Services.Abstractions;

namespace Test1Retake.Services;

public class MovieService(IUnitOfWork unitOfWork) : IMovieService
{
    public async Task<List<MovieWithDetailsResponse>> GetMoviesAsync(MovieFilterDto filter,
        CancellationToken token = default)
    {
        const string query = """
                             SELECT
                                 M.IdMovie,
                                 M.Name,
                                 M.ReleaseDate,
                                 AR.Name AS AgeRating,
                                 A.IdActor,
                                 A.Name,
                                 A.Surname,
                                 AM.CharacterName
                             FROM Movie M
                             JOIN AgeRating AR ON AR.IdRating = M.IdAgeRating
                             LEFT JOIN Actor_Movie AM ON AM.IdMovie = M.IdMovie
                             LEFT JOIN Actor A ON A.IdActor = AM.IdActor
                             WHERE (@from IS NULL OR M.ReleaseDate >= @from)
                               AND (@to IS NULL OR M.ReleaseDate <= @to)
                             ORDER BY M.ReleaseDate DESC;
                             """;

        var con = await unitOfWork.GetConnectionAsync();
        await using var cmd = con.CreateCommand();
        cmd.CommandText = query;

        cmd.Parameters.AddWithValue("@from", filter.From ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@to", filter.To ?? (object)DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync(token);

        var movies = new List<MovieWithDetailsResponse>();

        int? currentMovieId = null;
        MovieWithDetailsResponse? currentMovie = null;

        while (await reader.ReadAsync(token))
        {
            var movieId = reader.GetInt32(0);

            if (currentMovieId != movieId)
            {
                if (currentMovie is not null)
                    movies.Add(currentMovie.Value);

                currentMovieId = movieId;
                currentMovie = new MovieWithDetailsResponse(
                    Id: movieId,
                    Name: reader.GetString(1),
                    ReleaseDate: reader.GetDateTime(2),
                    AgeRating: reader.GetString(3),
                    Actors: []
                );
            }

            if (reader.IsDBNull(4) || currentMovie is null) continue;
            var actor = new ActorDto(
                Id: reader.GetInt32(4),
                Name: reader.GetString(5),
                Surname: reader.GetString(6),
                CharacterName: reader.GetString(7)
            );
                
            currentMovie = currentMovie.Value with
            {
                Actors = currentMovie.Value.Actors.Append(actor).ToList()
            };
        }

        if (currentMovie is not null)
            movies.Add(currentMovie.Value);

        return movies;
    }
    
    public async Task AssignActorToMovieAsync(AssignActorRequest request, CancellationToken token = default)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            var con = await unitOfWork.GetConnectionAsync();
            var tx = unitOfWork.Transaction;
            
            var movieExists = await EntityExistsAsync("Movie", "IdMovie", request.MovieId, con, token, tx);
            if (!movieExists)
                throw new MovieExceptions.MovieNotFoundException(request.MovieId);
            
            var actorExists = await EntityExistsAsync("Actor", "IdActor", request.ActorId, con, token, tx);
            if (!actorExists)
                throw new MovieExceptions.ActorNotFoundException(request.ActorId);
            
            const string query = """
                SELECT COUNT(1)
                FROM Actor_Movie
                WHERE IdMovie = @movieId AND IdActor = @actorId;
            """;

            await using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Transaction = tx;
                cmd.Parameters.AddWithValue("@movieId", request.MovieId);
                cmd.Parameters.AddWithValue("@actorId", request.ActorId);

                var alreadyAssigned = Convert.ToInt32(await cmd.ExecuteScalarAsync(token)) > 0;
                if (alreadyAssigned)
                    throw new MovieExceptions.ActorAlreadyAssignedException(request.MovieId, request.ActorId);
            }
            
            const string insert = """
                INSERT INTO Actor_Movie (IdMovie, IdActor, CharacterName)
                VALUES (@movieId, @actorId, @characterName);
            """;

            await using var insertCmd = con.CreateCommand();
            insertCmd.CommandText = insert;
            insertCmd.Transaction = tx;
            insertCmd.Parameters.AddWithValue("@movieId", request.MovieId);
            insertCmd.Parameters.AddWithValue("@actorId", request.ActorId);
            insertCmd.Parameters.AddWithValue("@characterName", request.nickname);

            await insertCmd.ExecuteNonQueryAsync(token);
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
    
    private static async Task<bool> EntityExistsAsync(
        string tableName,
        string idColumn,
        int id,
        SqlConnection con,
        CancellationToken token,
        SqlTransaction? transaction = null)
    {
        var query = $"""
                     SELECT IIF(EXISTS (
                         SELECT 1 FROM {tableName}
                         WHERE {idColumn} = @id
                     ), 1, 0);
                     """;

        await using var cmd = con.CreateCommand();
        cmd.CommandText = query;
    
        if (transaction is not null)
            cmd.Transaction = transaction;

        cmd.Parameters.AddWithValue("@id", id);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync(token)) == 1;
    }
}
