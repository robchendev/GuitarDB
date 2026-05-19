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

public class BrandRepository(DataContext context) : IBrandRepository
{
  public async Task<BrandDto?> GetByIdAsync(int id)
  {
    return await context.Brands
      .Where(g => g.Id == id)
      .Select(g => new BrandDto
      {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<BrandDto>> GetAllAsync(BrandSearchParams brandParams)
  {
    var query = context.Brands.AsQueryable();

    if (!string.IsNullOrWhiteSpace(brandParams.Name))
    {
      var normalizedName = brandParams.Name.ToLower();
      query = query
        .Where(g => g.Name.ToLower().Contains(normalizedName));
    }

    var dtoQuery = query.Select(g => new BrandDto
    {
      Id = g.Id,
      Name = g.Name,
      Description = g.Description,
    });
    dtoQuery = dtoQuery.OrderByDescending(g => g.Id);

    return await PagedList<BrandDto>.CreateAsync(dtoQuery, brandParams.Page, brandParams.PageSize);
  }

  public Brand Create(BrandRevision brandRevision)
  {
    var brand = new Brand
    {
      Name = brandRevision.Name,
      Description = brandRevision.Description,
    };

    context.Brands.Add(brand);
    brandRevision.Brand = brand;

    return brand;
  }

  public async Task<Brand?> UpdateAsync(BrandRevision brandRevision)
  {
    var brand = await context.Brands
      .Where(g => g.Id == brandRevision.BrandId)
      .SingleOrDefaultAsync();

    if (brand == null) return null;

    brand.Name = brandRevision.Name;
    brand.Description = brandRevision.Description;

    return brand;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var brand = await context.Brands
      .Where(g => g.Id == id)
      .SingleOrDefaultAsync();

    if (brand == null) return false;

    context.Brands.Remove(brand);

    return true;
  }

  // TODO: Add logic to check if brand exists via name + countryId
  public async Task<bool> ExistsAsync(BrandExistsParams @params)
  {
    string normalizedName = @params.Name.ToLower();

    var query = context.Brands.Where(g =>
      g.Name.ToLower() == normalizedName && @params.CountryId == g.CountryId
    );

    if (@params.ExcludeId.HasValue)
    {
      query = query.Where(g => g.Id != @params.ExcludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<bool> ExistsByIdAsync(int id)
  {
    return await context.Brands.AnyAsync(g => g.Id == id);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int brandId, bool userIsModerator = false)
  {
    var query = context.BrandRevisions.AsQueryable();

    if (!userIsModerator || revisionExcerptParams.CommittedOnly == true)
    {
      query = query.Where(gr => gr.RevisionMetadata.Status == RevisionStatus.Committed);
    }

    var dtoQuery = query
      .OrderByDescending(gr => gr.Id)
      .Where(gr => gr.BrandId == brandId)
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
    var query = context.BrandRevisions.AsQueryable();

    var dtoQuery = query
      .OrderByDescending(gr => gr.Id)
      .Where(gr => gr.RevisionMetadata.Status == RevisionStatus.Pending && gr.BrandId == null)
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

  public async Task<BrandRevisionSnapshotDto?> GetRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null)
  {
    var gr = await context.BrandRevisions
      .Include(g => g.RevisionMetadata)
        .ThenInclude(rm => rm.CreatedBy)
      .Include(g => g.RevisionMetadata)
        .ThenInclude(rm => rm.UpdatedBy)
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

    return new BrandRevisionSnapshotDto
    {
      Id = gr.Revision.Id,
      Version = gr.Revision.RevisionMetadata.Version,
      ParentRevisionId = gr.Revision.RevisionMetadata.ParentRevisionId,
      RowVersion = gr.Revision.RevisionMetadata.RowVersion,
      Status = gr.Revision.RevisionMetadata.Status.ToString(),
      Name = gr.Revision.Name,
      Description = gr.Revision.Description,
      Comment = gr.Revision.RevisionMetadata.Comment,
      CreatedAt = gr.Revision.RevisionMetadata.CreatedAt,
      UpdatedAt = gr.Revision.RevisionMetadata.UpdatedAt,
      CreatedBy = gr.Revision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = gr.Revision.RevisionMetadata.CreatedBy?.Id,
      UpdatedBy = gr.Revision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = gr.Revision.RevisionMetadata.UpdatedBy?.Id,
    };
  }

  public async Task<BrandRevision?> GetRevisionEntityAsync(int revisionId)
  {
    return await context.BrandRevisions.SingleOrDefaultAsync(g => g.Id == revisionId);
  }

  public async Task<RevisionMetadataVersioningDto?> GetLatestRevisionVersionAsync(int brandId)
  {
    var query = context.BrandRevisions.AsQueryable();

    return await query
      .Where(gr => gr.BrandId == brandId && gr.RevisionMetadata.Version != null)
      .OrderByDescending(gr => gr.RevisionMetadata.Version) // Version should never be null when Committed
      .Select(gr => new RevisionMetadataVersioningDto
      {
        Id = gr.Id,
        Version = gr.RevisionMetadata.Version,
      })
      .FirstOrDefaultAsync();
  }

  public BrandRevision CreateInitialRevision(
    RevisionMetadata revisionMetadata,
    BrandCreateRevisionDto createBrandRevisionDto)
  {
    var brandRevision = new BrandRevision(revisionMetadata.Id)
    {
      BrandId = null,
      Name = createBrandRevisionDto.Name,
      Description = string.IsNullOrWhiteSpace(createBrandRevisionDto.Description) ? null : createBrandRevisionDto.Description,
    };

    context.BrandRevisions.Add(brandRevision);

    return brandRevision;
  }

  public BrandRevision CreateRevision(int brandId, RevisionMetadata revisionMetadata, BrandCreateRevisionDto createBrandRevisionDto)
  {
    var brandRevision = new BrandRevision(revisionMetadata.Id)
    {
      BrandId = brandId,
      Name = createBrandRevisionDto.Name,
      Description = string.IsNullOrWhiteSpace(createBrandRevisionDto.Description) ? null : createBrandRevisionDto.Description,
    };

    context.BrandRevisions.Add(brandRevision);

    return brandRevision;
  }

  public async Task<BrandRevision?> UpdateRevisionAsync(int revisionId, BrandCreateRevisionDto createBrandRevisionDto)
  {
    var brandRevision = await context.BrandRevisions
      .Where(g => g.Id == revisionId)
      .SingleOrDefaultAsync();

    if (brandRevision == null) return null;

    brandRevision.Name = createBrandRevisionDto.Name;
    brandRevision.Description = createBrandRevisionDto.Description;

    return brandRevision;
  }
}
