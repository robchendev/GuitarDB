using Backend.DTOs.Abstract;
using Backend.DTOs.Interfaces;

namespace Backend.DTOs;

public class BrandRevisionSnapshotDto : BaseBrandDto, IRevisionSnapshotDto
{
  public required int Id { get; set; }
  public int? BrandId { get; set; }

  public string Comment { get; set; } = null!;
  public string Status { get; set; } = null!;
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
