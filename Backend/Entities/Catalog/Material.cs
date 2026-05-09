using Backend.Entities.Abstract;

namespace Backend.Entities.Catalog;

public class Material : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Sitka Spruce, 
  // Do not put ICollection here, Material does not need navigation to anything.
}
