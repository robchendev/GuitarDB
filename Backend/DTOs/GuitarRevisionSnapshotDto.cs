namespace Backend.DTOs;

public class GuitarRevisionSnapshotDto : GuitarDto
{
  public string Comment { get; set; } = null!;
  public string Status { get; set; } = null!;
  public int? GuitarId { get; set; }
  public int? Version { get; set; }
  public int? ParentRevisionId { get; set; }

  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  public string? CreatedBy { get; set; }
  public int? CreatedById { get; set; }
  public string? UpdatedBy { get; set; }
  public int? UpdatedById { get; set; }
}
