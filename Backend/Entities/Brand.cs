using Backend.Common.Interfaces;
using Backend.Entities.Abstract;
using Backend.Entities.Catalog;
using Backend.Entities.Revision;

namespace Backend.Entities;

public class Brand : BaseEntity, IHasId
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? CountryId { get; set; }
  public Country? Country { get; set; }

  public ICollection<BrandRevision> Revisions { get; set; } = [];
  public ICollection<Guitar> Guitars { get; set; } = [];
}
