using System.ComponentModel.DataAnnotations.Schema;
using Backend.Entities.Abstract;

namespace Backend.Entities.Revision;

public class GuitarRevision : BaseGuitarEntity
{
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public int Id { get; private set; } // Created to be the same as RevisionMetadata Id, so this cannot be autoincremented
  public RevisionMetadata RevisionMetadata { get; set; } = null!; // uses Id as FK

  public int? GuitarId { get; set; } // nullable because if initial revision, guitarId doesnt exist yet
  public Guitar? Guitar { get; set; } = null!;

  public GuitarRevision(int id)
  {
    if (id <= 0) throw new ArgumentException("Id must be a positive integer.", nameof(id));
    Id = id;
  }
}
