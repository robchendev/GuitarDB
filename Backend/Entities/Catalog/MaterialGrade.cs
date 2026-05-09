using Backend.Entities.Abstract;

namespace Backend.Entities.Catalog;

public class MaterialGrade : BaseEntity
{
  public string Name { get; set; } = string.Empty; // AA, AAAA, Master
  // Do not put ICollection here, MaterialGrade does not need navigation to anything.
}
