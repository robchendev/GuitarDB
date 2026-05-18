namespace Backend.DTOs;

public class RevisionMetadataDto
{
  public int Id { get; set; }
  public string Comment { get; set; } = null!;
  public string Status { get; set; } = null!;
  public int? Version { get; set; }
  public int? ParentRevisionId { get; set; }
  public uint RowVersion { get; set; }

  public string EntityType { get; set; } = default!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public string? CreatedBy { get; set; }
  public string? UpdatedBy { get; set; }
}
