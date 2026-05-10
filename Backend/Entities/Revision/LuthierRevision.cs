using System.ComponentModel.DataAnnotations.Schema;
using Backend.Entities.Abstract;

namespace Backend.Entities.Revision;

public class LuthierRevision : BaseLuthierEntity
{
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public int Id { get; private set; } // Created to be the same as RevisionMetadata Id, so this cannot be autoincremented
  public RevisionMetadata RevisionMetadata { get; set; } = null!; // uses Id as FK

  public int? LuthierId { get; set; }
  public Luthier? Luthier { get; set; } = null!;

  public LuthierRevision(int id)
  {
    if (id <= 0) throw new ArgumentException("Id must be a positive integer.", nameof(id));
    Id = id;
  }
}
