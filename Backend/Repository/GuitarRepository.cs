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

public class GuitarRepository(DataContext context) : IGuitarRepository
{
  public async Task<GuitarDto?> GetByIdAsync(int id)
  {
    return await context.Guitars
      .Where(g => g.Id == id)
      .Select(g => new GuitarDto
      {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
        Luthier = g.Luthier == null ? null : new LuthierExerptDto
        {
          Id = g.Luthier.Id,
          Name = g.Luthier.Name
        }
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<GuitarDto>> GetAllAsync(GuitarSearchParams guitarParams)
  {
    var query = context.Guitars.AsQueryable();

    if (!string.IsNullOrWhiteSpace(guitarParams.Name))
    {
      var normalizedName = guitarParams.Name.ToLower();
      query = query
        .Where(g => g.Name.ToLower().Contains(normalizedName));
    }

    var dtoQuery = query.Select(g => new GuitarDto
    {
      Id = g.Id,
      Name = g.Name,
      Description = g.Description,
    });
    dtoQuery = dtoQuery.OrderByDescending(g => g.Id);

    return await PagedList<GuitarDto>.CreateAsync(dtoQuery, guitarParams.Page, guitarParams.PageSize);
  }

  public Guitar Create(GuitarRevision guitarRevision)
  {
    var guitar = new Guitar
    {
      Name = guitarRevision.Name,
      Description = guitarRevision.Description,
      LuthierId = guitarRevision.LuthierId
    };

    context.Guitars.Add(guitar);
    guitarRevision.Guitar = guitar;

    return guitar;
  }

  public async Task<Guitar?> UpdateAsync(GuitarRevision guitarRevision)
  {
    var guitar = await context.Guitars
      .Where(g => g.Id == guitarRevision.GuitarId)
      .SingleOrDefaultAsync();

    if (guitar == null) return null;

    guitar.Name = guitarRevision.Name;
    guitar.Description = guitarRevision.Description;
    guitar.LuthierId = guitarRevision.LuthierId;

    return guitar;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var guitar = await context.Guitars
      .Where(g => g.Id == id)
      .SingleOrDefaultAsync();

    if (guitar == null) return false;

    context.Guitars.Remove(guitar);

    return true;
  }

  // TODO: Add logic to check if guitar exists via name + luthier or name + brand 
  public async Task<bool> ExistsAsync(GuitarExistsParams @params)
  {
    string normalizedName = @params.Name.ToLower();

    var query = context.Guitars.Where(g =>
      g.Name.ToLower() == normalizedName &&
      (@params.BrandId == g.BrandId || @params.LuthierId == g.LuthierId)
    );

    if (@params.ExcludeId.HasValue)
    {
      query = query.Where(g => g.Id != @params.ExcludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<bool> ExistsByIdAsync(int id)
  {
    return await context.Guitars.AnyAsync(g => g.Id == id);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int guitarId, bool userIsModerator = false)
  {
    var query = context.GuitarRevisions.AsQueryable();

    if (!userIsModerator || revisionExcerptParams.CommittedOnly == true)
    {
      query = query.Where(gr => gr.RevisionMetadata.Status == RevisionStatus.Committed);
    }

    var dtoQuery = query
      .OrderByDescending(gr => gr.Id)
      .Where(gr => gr.GuitarId == guitarId)
      .Select(gr => new RevisionMetadataExcerptDto
      {
        Id = gr.Id,
        Comment = gr.RevisionMetadata.Comment,
        Version = gr.RevisionMetadata.Version,
        ParentRevisionId = gr.RevisionMetadata.ParentRevisionId,
        Status = gr.RevisionMetadata.Status.ToString(),
        CreatedAt = gr.RevisionMetadata.CreatedAt,
        UpdatedAt = gr.RevisionMetadata.UpdatedAt,
        CreatedBy = gr.RevisionMetadata.CreatedBy == null ? null : gr.RevisionMetadata.CreatedBy.UserName
      });

    return await PagedList<RevisionMetadataExcerptDto>.CreateAsync(dtoQuery, revisionExcerptParams.Page, revisionExcerptParams.PageSize);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetNewRevisionExcerptsAsync(RevisionParams revisionExcerptParams)
  {
    var query = context.GuitarRevisions.AsQueryable();

    var dtoQuery = query
      .OrderByDescending(gr => gr.Id)
      .Where(gr => gr.RevisionMetadata.Status == RevisionStatus.Pending && gr.GuitarId == null)
      .Select(gr => new RevisionMetadataExcerptDto
      {
        Id = gr.Id,
        Comment = gr.RevisionMetadata.Comment,
        Version = gr.RevisionMetadata.Version,
        ParentRevisionId = gr.RevisionMetadata.ParentRevisionId,
        Status = gr.RevisionMetadata.Status.ToString()
      });

    return await PagedList<RevisionMetadataExcerptDto>.CreateAsync(dtoQuery, revisionExcerptParams.Page, revisionExcerptParams.PageSize);
  }

  public async Task<GuitarRevisionSnapshotDto?> GetRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null)
  {
    var gr = await context.GuitarRevisions
      .Include(g => g.RevisionMetadata)
        .ThenInclude(rm => rm.CreatedBy)
      .Include(g => g.RevisionMetadata)
        .ThenInclude(rm => rm.UpdatedBy)
      .Include(g => g.Luthier)
      .Where(gr => gr.Id == revisionId)
      .Select(gr => new
      {
        Revision = gr,
        RevisionStatus = gr.RevisionMetadata.Status,
        RevisionCreatorId = gr.RevisionMetadata.CreatedById,
      })
      .SingleOrDefaultAsync();

    if (gr == null) return null;

    bool isStatusCheckRequired = !userIsModerator && gr.RevisionStatus != RevisionStatus.Committed;
    bool isUserMismatch = userId == null || gr.RevisionCreatorId != userId;
    if (isStatusCheckRequired && isUserMismatch) return null;

    return new GuitarRevisionSnapshotDto
    {
      Id = gr.Revision.Id,
      Version = gr.Revision.RevisionMetadata.Version,
      GuitarId = gr.Revision.GuitarId,
      ParentRevisionId = gr.Revision.RevisionMetadata.ParentRevisionId,
      RowVersion = gr.Revision.RevisionMetadata.RowVersion,
      Status = gr.Revision.RevisionMetadata.Status.ToString(),
      Name = gr.Revision.Name,
      Description = gr.Revision.Description,
      Luthier = gr.Revision.Luthier == null ? null : new LuthierExerptDto
      {
        Id = gr.Revision.Luthier.Id,
        Name = gr.Revision.Luthier.Name
      },
      Comment = gr.Revision.RevisionMetadata.Comment,
      CreatedAt = gr.Revision.RevisionMetadata.CreatedAt,
      UpdatedAt = gr.Revision.RevisionMetadata.UpdatedAt,
      CreatedBy = gr.Revision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = gr.Revision.RevisionMetadata.CreatedBy?.Id,
      UpdatedBy = gr.Revision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = gr.Revision.RevisionMetadata.UpdatedBy?.Id,
    };
  }

  public async Task<GuitarRevision?> GetRevisionEntityAsync(int revisionId)
  {
    return await context.GuitarRevisions.SingleOrDefaultAsync(g => g.Id == revisionId);
  }

  public async Task<RevisionMetadataVersioningDto?> GetLatestRevisionVersionAsync(int guitarId)
  {
    var query = context.GuitarRevisions.AsQueryable();

    return await query
      .Where(gr => gr.GuitarId == guitarId && gr.RevisionMetadata.Version != null)
      .OrderByDescending(gr => gr.RevisionMetadata.Version) // Version should never be null when Committed
      .Select(gr => new RevisionMetadataVersioningDto
      {
        Id = gr.Id,
        Version = gr.RevisionMetadata.Version,
      })
      .FirstOrDefaultAsync();
  }

  public GuitarRevision CreateInitialRevision(
    RevisionMetadata revisionMetadata,
    GuitarCreateRevisionDto createGuitarRevisionDto)
  {
    var guitarRevision = new GuitarRevision(revisionMetadata.Id)
    {
      GuitarId = null,
      Name = createGuitarRevisionDto.Name,
      Description = string.IsNullOrWhiteSpace(createGuitarRevisionDto.Description) ? null : createGuitarRevisionDto.Description,
      LuthierId = createGuitarRevisionDto.LuthierId
    };

    context.GuitarRevisions.Add(guitarRevision);

    return guitarRevision;
  }

  public GuitarRevision CreateRevision(int guitarId, RevisionMetadata revisionMetadata, GuitarCreateRevisionDto createGuitarRevisionDto)
  {
    var guitarRevision = new GuitarRevision(revisionMetadata.Id)
    {
      GuitarId = guitarId,
      Name = createGuitarRevisionDto.Name,
      Description = string.IsNullOrWhiteSpace(createGuitarRevisionDto.Description) ? null : createGuitarRevisionDto.Description,
      LuthierId = createGuitarRevisionDto.LuthierId
    };

    context.GuitarRevisions.Add(guitarRevision);

    return guitarRevision;
  }

  public async Task<GuitarRevision?> UpdateRevisionAsync(int revisionId, GuitarCreateRevisionDto createGuitarRevisionDto)
  {
    var guitarRevision = await context.GuitarRevisions
      .Where(g => g.Id == revisionId)
      .SingleOrDefaultAsync();

    if (guitarRevision == null) return null;

    guitarRevision.Name = createGuitarRevisionDto.Name;
    guitarRevision.Description = createGuitarRevisionDto.Description;
    guitarRevision.LuthierId = createGuitarRevisionDto.LuthierId;

    return guitarRevision;
  }
}
