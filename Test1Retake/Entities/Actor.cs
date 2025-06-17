namespace Test1Retake.Entities;

public class Actor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    public List<ActorMovie> ActorMovies { get; set; } = [];
}