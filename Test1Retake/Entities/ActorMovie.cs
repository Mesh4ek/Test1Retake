namespace Test1Retake.Entities;

public class ActorMovie : BaseEntity
{
    public string CharacterName { get; set; } = string.Empty;
    
    public int IdMovie { get; set; }
    public Movie? Movie { get; set; }
    
    public int IdActor { get; set; }
    public Actor? Actor { get; set; }
}