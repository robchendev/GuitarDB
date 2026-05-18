using Backend.Common.Interfaces;

namespace Backend.DTOs.Interfaces;

public interface IRevisionSnapshotDto : IHasId
{
  public string Comment { get; set; }
  public string Status { get; set; }
  public int? Version { get; set; }
  public int? ParentRevisionId { get; set; }
  public uint RowVersion { get; set; }

  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  public string? CreatedBy { get; set; }
  public int? CreatedById { get; set; }
  public string? UpdatedBy { get; set; }
  public int? UpdatedById { get; set; }
}
