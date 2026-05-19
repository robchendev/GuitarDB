using Backend.Entities.Catalog;

namespace Backend.Entities.Abstract;

public abstract class BaseBrandEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? CountryId { get; set; }
  public Country? Country { get; set; }
}
