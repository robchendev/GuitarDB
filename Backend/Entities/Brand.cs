using Backend.Entities.Abstract;
using Backend.Entities.Catalog;

namespace Backend.Entities;

public class Brand : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? CountryId { get; set; }
  public Country? Country { get; set; }

  public ICollection<Guitar> Guitars { get; set; } = [];
}
