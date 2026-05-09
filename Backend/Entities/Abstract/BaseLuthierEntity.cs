using Backend.Entities.Catalog;

namespace Backend.Entities.Abstract;

public abstract class BaseLuthierEntity
{
  public string Name { get; set; } = string.Empty;
  // Date of Birth
  public string? Description { get; set; }
  public string? CountryId { get; set; }
  public Country? Country { get; set; }
}
