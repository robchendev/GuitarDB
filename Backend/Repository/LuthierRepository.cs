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
  public async Task<LuthierDto?> GetByIdAsync(int id)
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

  public async Task<PagedList<LuthierDto>> GetAllAsync(LuthierSearchParams luthierParams)
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

  public Luthier Create(LuthierRevision luthierRevision)
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

  public async Task<Luthier?> UpdateAsync(LuthierRevision luthierRevision)
  {
    var luthier = await context.Luthiers
      .Where(l => l.Id == luthierRevision.LuthierId)
      .SingleOrDefaultAsync();

    if (luthier == null) return null;

    luthier.Name = luthierRevision.Name;
    luthier.Description = luthierRevision.Description;

    return luthier;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var luthier = await context.Luthiers
      .Where(l => l.Id == id)
      .SingleOrDefaultAsync();

    if (luthier == null) return false;

    context.Luthiers.Remove(luthier);

    return true;
  }

  // TODO: Add logic to check if luthier exists via name + country  
  // get rid of brandId here
  public async Task<bool> ExistsAsync(LuthierExistsParams @params)
  {
    string normalizedName = @params.Name.ToLower();

    var query = context.Luthiers.Where(l =>
      l.Name.ToLower() == normalizedName
    );

    if (@params.ExcludeId.HasValue)
    {
      query = query.Where(l => l.Id != @params.ExcludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<bool> ExistsByIdAsync(int id)
  {
    return await context.Luthiers.AnyAsync(l => l.Id == id);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int luthierId, bool userIsModerator = false)
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

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetNewRevisionExcerptsAsync(RevisionParams revisionExcerptParams)
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

  public async Task<LuthierRevisionSnapshotDto?> GetRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null)
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
      RowVersion = lr.Revision.RevisionMetadata.RowVersion,
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

  public async Task<LuthierRevision?> GetRevisionEntityAsync(int revisionId)
  {
    return await context.LuthierRevisions.SingleOrDefaultAsync(l => l.Id == revisionId);
  }

  public async Task<RevisionMetadataVersioningDto?> GetLatestRevisionVersionAsync(int luthierId)
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

  public LuthierRevision CreateInitialRevision(
    RevisionMetadata revisionMetadata,
    LuthierCreateRevisionDto createLuthierRevisionDto)
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

  public LuthierRevision CreateRevision(int luthierId, RevisionMetadata revisionMetadata, LuthierCreateRevisionDto createLuthierRevisionDto)
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

  public async Task<LuthierRevision?> UpdateRevisionAsync(int revisionId, LuthierCreateRevisionDto createLuthierRevisionDto)
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
