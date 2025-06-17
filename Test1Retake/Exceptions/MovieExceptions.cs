namespace Test1Retake.Exceptions;

public class MovieExceptions
{
    public class MovieNotFoundException(int id)
        : BaseExceptions.NotFoundException($"Movie with id: {id} does not exist");
    
    public class ActorNotFoundException(int id)
        : BaseExceptions.NotFoundException($"Actor with id: {id} does not exist");
    
    public class ActorAlreadyAssignedException(int movieId, int actorId)
        : BaseExceptions.ConflictException($"Actor with id {actorId} is already assigned to movie {movieId}") { }
  
}
