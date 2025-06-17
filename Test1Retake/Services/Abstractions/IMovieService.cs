using Test1Retake.Contracts.Requests;
using Test1Retake.Contracts.Responses;

namespace Test1Retake.Services.Abstractions;

public interface IMovieService
{
    Task <List<MovieWithDetailsResponse>> GetMoviesAsync(MovieFilterDto filter, CancellationToken token = default);
    
    Task AssignActorToMovieAsync(AssignActorRequest request, CancellationToken token = default);
}

