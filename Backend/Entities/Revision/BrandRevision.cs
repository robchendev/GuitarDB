using System.ComponentModel.DataAnnotations.Schema;
using Backend.Entities.Abstract;

namespace Backend.Entities.Revision;

public class BrandRevision : BaseBrandEntity, IRevisionEntity
{
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public int Id { get; private set; } // Created to be the same as RevisionMetadata Id, so this cannot be autoincremented
  public RevisionMetadata RevisionMetadata { get; set; } = null!; // uses Id as FK

  public int? BrandId { get; set; }
  public Brand? Brand { get; set; } = null!;

  public BrandRevision(int id)
  {
    if (id <= 0) throw new ArgumentException("Id must be a positive integer.", nameof(id));
    Id = id;
  }
}
