using System.ComponentModel.DataAnnotations;

namespace Test1Retake.Contracts.Requests;

public class AssignActorRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int MovieId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ActorId { get; set; }

    [Required]
    [StringLength(100)]
    public string nickname { get; set; } = string.Empty;
}