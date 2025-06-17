namespace Test1Retake.Entities;

public class Movie : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    
    public int IdAgeRating { get; set; }
    public AgeRating? AgeRating { get; set; }
    
    public List<ActorMovie> ActorMovies { get; set; } = [];
}