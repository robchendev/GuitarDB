using System.ComponentModel.DataAnnotations;
using Backend.DTOs.Abstract;

namespace Backend.DTOs;

public class GuitarCreateRevisionDto : BaseRevisionDto
{
  [Required(ErrorMessage = "Name is required.")]
  [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long.")]
  public required string Name { get; set; } = string.Empty;

  [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
  public string? Description { get; set; }

  [Range(1, int.MaxValue, ErrorMessage = "Luthier ID must be greater than 0.")]
  public int? LuthierId { get; set; }

  // Can add more constraints later as we start handling more data to edit
  // TODO: Once add BrandId, LuthierId into this DTO, edit all uses of GuitarExistsAsync to include those checks
}
