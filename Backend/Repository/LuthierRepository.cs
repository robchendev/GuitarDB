using System.Reflection;
using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;
using Backend.Enums;
using Backend.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class LuthierRepository(DataContext context) : ILuthierRepository
{
  public async Task<LuthierDto?> GetLuthierByIdAsync(int id)
  {
    return await context.Luthiers
      .Where(l => l.Id == id)
      .Select(l => new LuthierDto
      {
        Id = l.Id,
        Name = l.Name,
        Description = l.Description
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<LuthierDto>> GetLuthiersAsync(LuthierParams luthierParams)
  {
    var query = context.Luthiers.AsQueryable();

    if (!string.IsNullOrWhiteSpace(luthierParams.Name))
    {
      var normalizedName = luthierParams.Name.ToLower();
      query = query
        .Where(l => l.Name.ToLower().Contains(normalizedName));
    }

    var dtoQuery = query.Select(l => new LuthierDto
    {
      Id = l.Id,
      Name = l.Name,
      Description = l.Description,
    });
    dtoQuery = dtoQuery.OrderByDescending(l => l.Id);

    return await PagedList<LuthierDto>.CreateAsync(dtoQuery, luthierParams.Page, luthierParams.PageSize);
  }

  public Luthier CreateLuthier(LuthierRevision luthierRevision)
  {
    var luthier = new Luthier
    {
      Name = luthierRevision.Name,
      Description = luthierRevision.Description,
    };

    context.Luthiers.Add(luthier);
    luthierRevision.Luthier = luthier;

    return luthier;
  }

  public async Task<Luthier?> UpdateLuthierAsync(LuthierRevision luthierRevision)
  {
    var luthier = await context.Luthiers
      .Where(l => l.Id == luthierRevision.LuthierId)
      .SingleOrDefaultAsync();

    if (luthier == null) return null;

    luthier.Name = luthierRevision.Name;
    luthier.Description = luthierRevision.Description;

    return luthier;
  }

  public async Task<bool> DeleteLuthierAsync(int id)
  {
    var luthier = await context.Luthiers
      .Where(l => l.Id == id)
      .SingleOrDefaultAsync();

    if (luthier == null) return false;

    context.Luthiers.Remove(luthier);

    return true;
  }

  // TODO: Add logic to check if luthier exists via name + country  
  public async Task<bool> LuthierExistsAsync(string name, int? brandId = null, int? luthierId = null, int? excludeId = null)
  {
    string normalizedName = name.ToLower();

    var query = context.Luthiers.Where(l =>
      l.Name.ToLower() == normalizedName
    );

    if (excludeId.HasValue)
    {
      query = query.Where(l => l.Id != excludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<bool> LuthierExistsByIdAsync(int id)
  {
    return await context.Luthiers.AnyAsync(l => l.Id == id);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetLuthierRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int luthierId, bool userIsModerator = false)
  {
    var query = context.LuthierRevisions.AsQueryable();

    if (!userIsModerator || revisionExcerptParams.CommittedOnly == true)
    {
      query = query.Where(lr => lr.RevisionMetadata.Status == RevisionStatus.Committed);
    }

    var dtoQuery = query
      .OrderByDescending(lr => lr.Id)
      .Where(lr => lr.LuthierId == luthierId)
      .Select(lr => new RevisionMetadataExcerptDto
      {
        Id = lr.Id,
        Comment = lr.RevisionMetadata.Comment,
        Version = lr.RevisionMetadata.Version,
        ParentRevisionId = lr.RevisionMetadata.ParentRevisionId,
        Status = lr.RevisionMetadata.Status.ToString(),
        CreatedAt = lr.RevisionMetadata.CreatedAt,
        UpdatedAt = lr.RevisionMetadata.UpdatedAt,
        CreatedBy = lr.RevisionMetadata.CreatedBy == null ? null : lr.RevisionMetadata.CreatedBy.UserName
      });

    return await PagedList<RevisionMetadataExcerptDto>.CreateAsync(dtoQuery, revisionExcerptParams.Page, revisionExcerptParams.PageSize);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetNewLuthierRevisionExcerptsAsync(RevisionParams revisionExcerptParams)
  {
    var query = context.LuthierRevisions.AsQueryable();

    var dtoQuery = query
      .OrderByDescending(lr => lr.Id)
      .Where(lr => lr.RevisionMetadata.Status == RevisionStatus.Pending && lr.LuthierId == null)
      .Select(lr => new RevisionMetadataExcerptDto
      {
        Id = lr.Id,
        Comment = lr.RevisionMetadata.Comment,
        Version = lr.RevisionMetadata.Version,
        ParentRevisionId = lr.RevisionMetadata.ParentRevisionId,
        Status = lr.RevisionMetadata.Status.ToString()
      });

    return await PagedList<RevisionMetadataExcerptDto>.CreateAsync(dtoQuery, revisionExcerptParams.Page, revisionExcerptParams.PageSize);
  }

  public async Task<LuthierRevisionSnapshotDto?> GetLuthierRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null)
  {
    var lr = await context.LuthierRevisions
      .Include(l => l.RevisionMetadata)
        .ThenInclude(rm => rm.CreatedBy)
      .Include(l => l.RevisionMetadata)
        .ThenInclude(rm => rm.UpdatedBy)
      .Where(lr => lr.Id == revisionId)
      .Select(lr => new
      {
        Revision = lr,
        RevisionStatus = lr.RevisionMetadata.Status,
        RevisionCreatorId = lr.RevisionMetadata.CreatedById,
      })
      .SingleOrDefaultAsync();

    if (lr == null) return null;

    bool isStatusCheckRequired = !userIsModerator && lr.RevisionStatus != RevisionStatus.Committed;
    bool isUserMismatch = userId == null || lr.RevisionCreatorId != userId;
    if (isStatusCheckRequired && isUserMismatch) return null;

    return new LuthierRevisionSnapshotDto
    {
      Id = lr.Revision.Id,
      Version = lr.Revision.RevisionMetadata.Version,
      LuthierId = lr.Revision.LuthierId,
      ParentRevisionId = lr.Revision.RevisionMetadata.ParentRevisionId,
      Iteration = lr.Revision.RevisionMetadata.Iteration,
      Status = lr.Revision.RevisionMetadata.Status.ToString(),
      Name = lr.Revision.Name,
      Description = lr.Revision.Description,
      Comment = lr.Revision.RevisionMetadata.Comment,
      CreatedAt = lr.Revision.RevisionMetadata.CreatedAt,
      UpdatedAt = lr.Revision.RevisionMetadata.UpdatedAt,
      CreatedBy = lr.Revision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = lr.Revision.RevisionMetadata.CreatedBy?.Id,
      UpdatedBy = lr.Revision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = lr.Revision.RevisionMetadata.UpdatedBy?.Id,
    };
  }

  public async Task<LuthierRevision?> GetLuthierRevisionEntityAsync(int revisionId)
  {
    return await context.LuthierRevisions.SingleOrDefaultAsync(l => l.Id == revisionId);
  }

  public async Task<RevisionMetadataVersioningDto?> GetLatestLuthierRevisionVersionAsync(int luthierId)
  {
    var query = context.LuthierRevisions.AsQueryable();

    return await query
      .Where(lr => lr.LuthierId == luthierId && lr.RevisionMetadata.Version != null)
      .OrderByDescending(lr => lr.RevisionMetadata.Version) // Version should never be null when Committed
      .Select(lr => new RevisionMetadataVersioningDto
      {
        Id = lr.Id,
        Version = lr.RevisionMetadata.Version,
      })
      .FirstOrDefaultAsync();
  }

  public LuthierRevision CreateInitialLuthierRevisionAsync(
    RevisionMetadata revisionMetadata,
    CreateLuthierRevisionDto createLuthierRevisionDto)
  {
    var luthierRevision = new LuthierRevision(revisionMetadata.Id)
    {
      LuthierId = null,
      Name = createLuthierRevisionDto.Name,
      Description = string.IsNullOrWhiteSpace(createLuthierRevisionDto.Description) ? null : createLuthierRevisionDto.Description,
    };

    context.LuthierRevisions.Add(luthierRevision);

    return luthierRevision;
  }

  public LuthierRevision CreateLuthierRevisionAsync(int luthierId, RevisionMetadata revisionMetadata, CreateLuthierRevisionDto createLuthierRevisionDto)
  {
    var luthierRevision = new LuthierRevision(revisionMetadata.Id)
    {
      LuthierId = luthierId,
      Name = createLuthierRevisionDto.Name,
      Description = string.IsNullOrWhiteSpace(createLuthierRevisionDto.Description) ? null : createLuthierRevisionDto.Description,
    };

    context.LuthierRevisions.Add(luthierRevision);

    return luthierRevision;
  }

  public async Task<LuthierRevision?> UpdateLuthierRevisionAsync(int revisionId, CreateLuthierRevisionDto createLuthierRevisionDto)
  {
    var luthierRevision = await context.LuthierRevisions
      .Where(l => l.Id == revisionId)
      .SingleOrDefaultAsync();

    if (luthierRevision == null) return null;

    luthierRevision.Name = createLuthierRevisionDto.Name;
    luthierRevision.Description = createLuthierRevisionDto.Description;

    return luthierRevision;
  }
}
