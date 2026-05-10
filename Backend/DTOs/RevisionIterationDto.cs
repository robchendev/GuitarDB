using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RevisionIterationDto
{
  [Range(1, int.MaxValue, ErrorMessage = "Iteration must be at least 1.")]
  public required int Iteration { get; set; }
}
