using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateGuitarRevisionDto
{
  [Required(ErrorMessage = "Name is required.")]
  [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long.")]
  public required string Name { get; set; } = string.Empty;

  [Required(ErrorMessage = "Comment is required.")]
  [MaxLength(300, ErrorMessage = "Comment must be at most 300 characters long.")]
  public required string Comment { get; set; } = string.Empty;

  [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
  public string? Description { get; set; }

  // Can add more constraints later as we start handling more data to edit
}
