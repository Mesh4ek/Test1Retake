namespace Test1Retake.Entities;

public class AgeRating : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public List<Movie> Movies { get; set; } = [];
}