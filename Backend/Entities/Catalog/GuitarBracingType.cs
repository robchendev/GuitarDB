using Backend.Entities.Abstract;

namespace Backend.Entities.Catalog;

// X, V, etc
public class GuitarBracingType : BaseEntity
{
  public string Name { get; set; } = string.Empty;

  public ICollection<Guitar> Guitars { get; set; } = [];
}
