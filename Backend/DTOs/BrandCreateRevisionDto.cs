using System.ComponentModel.DataAnnotations;
using Backend.DTOs.Abstract;

namespace Backend.DTOs;

public class BrandCreateRevisionDto : BaseRevisionDto
{
  [Required(ErrorMessage = "Name is required.")]
  [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long.")]
  public required string Name { get; set; } = string.Empty;

  [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
  public string? Description { get; set; }

  [MinLength(2, ErrorMessage = "Country ID must be exactly 2 characters long.")]
  [MaxLength(2, ErrorMessage = "Country ID must be exactly 2 characters long.")]
  public string? CountryId { get; set; }

  // Can add more constraints later as we start handling more data to edit
}
