using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities.Revision;
using Backend.Enums;
using Backend.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class RevisionMetadataRepository(DataContext context) : IRevisionMetadataRepository
{
  public async Task<RevisionMetadataDto?> GetRevisionMetadataAsync(int id)
  {
    return await context.RevisionMetadatas
      .Where(rm => rm.Id == id)
      .Select(rm => new RevisionMetadataDto
      {
        Id = rm.Id,
        Status = rm.Status.ToString(),
        ParentRevisionId = rm.ParentRevisionId,
        Version = rm.Version,
        Comment = rm.Comment,
        EntityType =
          rm.GuitarRevision != null ? "Guitar" :
          rm.LuthierRevision != null ? "Luthier" :
          // rm.AnotherEntityRevisions != null ? "AnotherEntity" :
          "Unknown",
        CreatedAt = rm.CreatedAt,
        CreatedBy = rm.CreatedBy == null ? null : rm.CreatedBy.UserName,
        UpdatedAt = rm.UpdatedAt,
        UpdatedBy = rm.UpdatedBy == null ? null : rm.UpdatedBy.UserName,
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetUserRevisionMetadatasAsync(UserRevisionParams userRevisionParams, int userId, List<RevisionStatus> statuses)
  {
    var query = context.RevisionMetadatas
      .Include(rm => rm.CreatedBy)
      .Include(rm => rm.UpdatedBy)
      .Where(rm => rm.CreatedById == userId)
      .AsQueryable();

    if (statuses != null && statuses.Count > 0)
    {
      var allStatusValues = Enum.GetValues<RevisionStatus>().ToList();
      if (statuses.Count != allStatusValues.Count)
      {
        query = query.Where(rm => statuses.Contains(rm.Status));
      }
    }

    var dtoQuery = query
      .ProjectToRevisionMetadataWithEntityIdentifierDto()
      .OrderByDescending(r => r.Id);

    return await PagedList<RevisionMetadataWithEntityIdentifierDto>.CreateAsync(dtoQuery, userRevisionParams.Page, userRevisionParams.PageSize);
  }

  public async Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams)
  {
    var query = context.RevisionMetadatas
      .Include(rm => rm.CreatedBy)
      .Include(rm => rm.UpdatedBy)
      .Where(rm => rm.Status == RevisionStatus.Pending)
      .AsQueryable();

    var dtoQuery = query
      .ProjectToRevisionMetadataWithEntityIdentifierDto()
      .OrderByDescending(r => r.Id);

    return await PagedList<RevisionMetadataWithEntityIdentifierDto>.CreateAsync(dtoQuery, revisionParams.Page, revisionParams.PageSize);
  }

  public async Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetCommittedRevisionMetadatasAsync(RevisionParams revisionParams, int userId)
  {
    var query = context.RevisionMetadatas
      .Include(rm => rm.CreatedBy)
      .Include(rm => rm.UpdatedBy)
      .Where(rm => rm.CreatedById == userId && rm.Status == RevisionStatus.Committed)
      .AsQueryable();

    var dtoQuery = query
      .ProjectToRevisionMetadataWithEntityIdentifierDto()
      .OrderByDescending(r => r.Id);

    return await PagedList<RevisionMetadataWithEntityIdentifierDto>.CreateAsync(dtoQuery, revisionParams.Page, revisionParams.PageSize);
  }

  public RevisionMetadata CreateRevisionMetadataAsync(string comment, int userId, int? parentRevisionId)
  {
    var revisionMetadata = new RevisionMetadata
    {
      Status = RevisionStatus.Pending,
      ParentRevisionId = parentRevisionId,
      Comment = comment,
      CreatedAt = DateTime.UtcNow,
      CreatedById = userId
    };

    context.RevisionMetadatas.Add(revisionMetadata);

    return revisionMetadata;
  }

  public async Task<RevisionMetadata?> UpdateRevisionMetadataAsync(int revisionId, string comment, int userId)
  {
    var revisionMetadata = await context.RevisionMetadatas
      .Where(rm => rm.Id == revisionId)
      .SingleOrDefaultAsync();

    if (revisionMetadata == null) return null;

    revisionMetadata.Comment = comment;
    revisionMetadata.UpdatedAt = DateTime.UtcNow;
    revisionMetadata.UpdatedById = userId;

    return revisionMetadata;
  }

  public async Task<bool> ApprovePendingRevisionMetadataAsync(int id, int approverUserId, int? latestVersionId = null)
  {
    var revisionMetadata = await context.RevisionMetadatas
      .Where(rm => rm.Id == id)
      .SingleOrDefaultAsync();

    if (revisionMetadata == null) return false;

    revisionMetadata.Status = RevisionStatus.Committed;
    revisionMetadata.UpdatedAt = DateTime.UtcNow;
    revisionMetadata.UpdatedById = approverUserId;

    if (latestVersionId == null) revisionMetadata.Version = 1;
    else if (latestVersionId <= 0) return false;
    else revisionMetadata.Version = latestVersionId + 1;

    return true;
  }

  public async Task<bool> RejectPendingRevisionMetadataAsync(int id, int rejecterUserId)
  {
    var revisionMetadata = await context.RevisionMetadatas
      .Where(rm => rm.Id == id)
      .SingleOrDefaultAsync();

    if (revisionMetadata == null) return false;

    revisionMetadata.Status = RevisionStatus.Rejected;
    revisionMetadata.UpdatedAt = DateTime.UtcNow;
    revisionMetadata.UpdatedById = rejecterUserId;

    return true;
  }

  public async Task<int> AdoptPendingRevisionMetadatasAsync(int oldParentRevisionId, int newParentRevisionId, int approverUserId)
  {
    var childRevisions = await context.RevisionMetadatas
      .Where(rm => rm.ParentRevisionId == oldParentRevisionId && (rm.Status == RevisionStatus.Pending || rm.Status == RevisionStatus.Draft))
      .ToListAsync();

    if (childRevisions.Count == 0) return 0;

    foreach (var childRevision in childRevisions)
    {
      childRevision.ParentRevisionId = newParentRevisionId;
      childRevision.UpdatedAt = DateTime.UtcNow;
      childRevision.UpdatedById = approverUserId;
    }

    return childRevisions.Count;
  }
}

public static class RevisionMetadataExtensions
{
  public static IQueryable<RevisionMetadataWithEntityIdentifierDto> ProjectToRevisionMetadataWithEntityIdentifierDto(
      this IQueryable<RevisionMetadata> query)
  {
    return query.Select(rm => new RevisionMetadataWithEntityIdentifierDto
    {
      Id = rm.Id,
      Status = rm.Status.ToString(),
      ParentRevisionId = rm.ParentRevisionId,
      Version = rm.Version,
      Comment = rm.Comment,
      EntityType =
        rm.GuitarRevision != null ? "Guitar" :
        rm.LuthierRevision != null ? "Luthier" :
        "Unknown",
      EntityName =
        rm.GuitarRevision != null ? rm.GuitarRevision.Name :
        rm.LuthierRevision != null ? rm.LuthierRevision.Name :
        "Unknown",
      EntityId =
        rm.GuitarRevision != null ? rm.GuitarRevision.GuitarId :
        rm.LuthierRevision != null ? rm.LuthierRevision.LuthierId :
        null,
      CreatedAt = rm.CreatedAt,
      CreatedBy = rm.CreatedBy == null ? null : rm.CreatedBy.UserName,
      UpdatedAt = rm.UpdatedAt,
      UpdatedBy = rm.UpdatedBy == null ? null : rm.UpdatedBy.UserName,
    });
  }
}