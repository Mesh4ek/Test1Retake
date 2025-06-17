namespace Test1Retake.Contracts.Responses;

public record struct MovieWithDetailsResponse(
    int Id,
    string Name,
    DateTime ReleaseDate,
    string AgeRating,
    List<ActorDto> Actors
);