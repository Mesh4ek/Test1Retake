namespace Test1Retake.Contracts.Responses;

public record struct ActorDto(
    int Id,
    string Name,
    string Surname,
    string CharacterName
);