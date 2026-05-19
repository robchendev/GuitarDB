using System.ComponentModel.DataAnnotations;
using Backend.Entities.Abstract;
using Backend.Enums;

namespace Backend.Entities.Revision;

public class RevisionMetadata : BaseAuditableEntity
{
  // RevisionMetadata.Id are the same as GuitarRevision.Id, etc.
  // Meaning we could have this
  // GuitarRevision     1 RevisionMetadata 1
  // LuthierRevision    2 RevisionMetadata 2
  // TechniqueRevision  3 RevisionMetadata 3
  public RevisionStatus Status { get; set; }
  public int? ParentRevisionId { get; set; }
  public int? Version { get; set; }
  public string Comment { get; set; } = string.Empty;

  // concurrency tracking
  public uint RowVersion { get; set; }

  public RevisionMetadata ParentRevision { get; set; } = null!;

  // Navigation to exactly one of the revision types
  public GuitarRevision? GuitarRevision { get; set; } = null!;
  public LuthierRevision? LuthierRevision { get; set; } = null!;
  public BrandRevision? BrandRevision { get; set; } = null!;
}
