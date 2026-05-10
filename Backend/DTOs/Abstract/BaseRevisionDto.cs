using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Abstract;

public abstract class BaseRevisionDto
{
  [Required(ErrorMessage = "Comment is required.")]
  [MaxLength(300, ErrorMessage = "Comment must be at most 300 characters long.")]
  public required string Comment { get; set; } = string.Empty;
}