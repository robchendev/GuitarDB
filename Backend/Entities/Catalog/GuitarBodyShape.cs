using Backend.Entities.Abstract;

namespace Backend.Entities.Catalog;

// Stratocaster, Grand Auditorium, 000, Custom (eg Natasha's custom body shapes)
public class GuitarBodyShape : BaseEntity
{
  public string Name { get; set; } = string.Empty;

  public ICollection<Guitar> Guitars { get; set; } = [];
}
