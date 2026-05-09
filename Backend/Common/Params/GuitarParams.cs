using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Params;

public class GuitarParams : PaginationParams, IValidatableObject
{
  [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
  public string? Name { get; set; }

  [MaxLength(200, ErrorMessage = "Address cannot be more than 200 characters long.")]
  public string? Address { get; set; }

  // PostGIS search
  [Range(typeof(double), "-90", "90", ErrorMessage = "Latitude must be between -90 and 90.")]
  public double? Lat { get; set; }

  [Range(typeof(double), "-180", "180", ErrorMessage = "Latitude must be between -90 and 90.")]
  public double? Long { get; set; }

  [Range(typeof(double), "0.1", "15000", ErrorMessage = "Radius must be between 0.1km and 15000km.")]
  public double? Radius { get; set; }

  public IEnumerable<ValidationResult> Validate(ValidationContext _)
  {
    var searchValues = new bool[] { Lat.HasValue, Long.HasValue, Radius.HasValue };
    var requiredCount = searchValues.Count(b => b);

    if (requiredCount > 0 && requiredCount < 3)
    {
      yield return new ValidationResult(
          "Latitude, Longitude, and Radius must all be provided together for a spatial search.",
          null
      );
    }
  }
}
