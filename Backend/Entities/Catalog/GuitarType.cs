using Backend.Entities.Abstract;

namespace Backend.Entities.Catalog;

public class GuitarType : BaseEntity
{
  public string Name { get; set; } = string.Empty;

  public ICollection<Guitar> Guitars { get; set; } = [];
}
