using Backend.Entities.Abstract;

namespace Backend.Entities.Catalog;

public class MaterialConstruction : BaseEntity
{
  public string Name { get; set; } = string.Empty; // solid, layered, laminate
  // Do not put ICollection here, MaterialConstruction does not need navigation to anything.
}
