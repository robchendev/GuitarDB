using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;
using Backend.Enums;
using Backend.Extensions;
using Backend.Interfaces;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Services.Abstract;

namespace Backend.Services;

public class BrandService : RevisableEntityService<
  int,
  Brand,
  BrandDto,
  BrandSearchParams,
  BrandExistsParams,
  BrandRevision,
  BrandRevisionSnapshotDto,
  BrandCreateRevisionDto
>, IBrandService
{
  public BrandService(IUnitOfWork unitOfWork) : base(unitOfWork, "Brand") { }

  protected override IRevisableEntityRepository<
    int,
    Brand,
    BrandDto,
    BrandSearchParams,
    BrandExistsParams,
    BrandRevision,
    BrandRevisionSnapshotDto,
    BrandCreateRevisionDto
  >
    GetRepository() => UnitOfWork.BrandRepository;

  protected override int? GetEntityId(BrandRevision revisionEntity) => revisionEntity.BrandId;

  protected override string GetEntityName(BrandDto entity) => entity.Name;

  public override async Task<ServiceResult<BrandRevisionSnapshotDto>> CreateInitialRevisionAsync(BrandCreateRevisionDto createBrandRevisionDto, ClaimsPrincipal userClaims)
  {
    if (string.IsNullOrWhiteSpace(createBrandRevisionDto.Name))
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, "Name must be provided");

    var brandExistsParams = new BrandExistsParams
    {
      Name = createBrandRevisionDto.Name
    };
    if (await UnitOfWork.BrandRepository.ExistsAsync(brandExistsParams))
    {
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Brand '{createBrandRevisionDto.Name}' already exists");
    }

    // TODO: implement after country
    // if (createBrandRevisionDto.CountryId != null && !await UnitOfWork.CountryRepository.ExistsByIdAsync(createBrandRevisionDto.CountryId))
    //   return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Country ID {createBrandRevisionDto.CountryId} does not exist.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = UnitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createBrandRevisionDto.Comment, userId, null);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(500, "Unable to create Entity Revision Metadata for initial brand.");

    var brandRevision = UnitOfWork.BrandRepository.CreateInitialRevision(revisionMetadata, createBrandRevisionDto);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(500, "Could not create initial brand revision");

    var brandRevisionSnapshotDto = new BrandRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = brandRevision.Id,
      BrandId = brandRevision.BrandId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      Name = brandRevision.Name,
      Description = brandRevision.Description,
      Country = brandRevision.Country == null ? null : new CountryExerptDto
      {
        Id = brandRevision.Country.Id,
        Name = brandRevision.Country.Name
      }
    };

    return ServiceResult<BrandRevisionSnapshotDto>.Success(200, brandRevisionSnapshotDto);
  }

  public override async Task<ServiceResult<BrandRevisionSnapshotDto>> CreateRevisionAsync(int id, BrandCreateRevisionDto createBrandRevisionDto, ClaimsPrincipal userClaims)
  {
    var currentBrand = await UnitOfWork.BrandRepository.GetByIdAsync(id);

    if (currentBrand == null)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Brand ID '{id}' does not exist");

    var brandExistsParams = new BrandExistsParams
    {
      Name = createBrandRevisionDto.Name,
      ExcludeId = id
    };
    if (!string.IsNullOrWhiteSpace(createBrandRevisionDto.Name) && await UnitOfWork.BrandRepository.ExistsAsync(brandExistsParams))
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Brand '{createBrandRevisionDto.Name}' already exists.");

    // TODO: After implementing Country
    // if (createBrandRevisionDto.CountryId != null && !await UnitOfWork.CountryRepository.ExistsByIdAsync(createBrandRevisionDto.CountryId))
    //   return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Country ID {createBrandRevisionDto.Country} does not exist.");

    // Should make a revision, not update brand.
    // CreateBrandRevisionAsync will be used when revision is approved and committed
    // Since we're making BrandRevision AND RevisionEntity, unit of work would help here

    bool hasChanges =
      currentBrand.Name != createBrandRevisionDto.Name ||
      currentBrand.Description != createBrandRevisionDto.Description ||
      currentBrand.Country?.Id != createBrandRevisionDto.CountryId;

    if (!hasChanges)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"No changes detected for Brand ID '{id}'. Revision not created.");

    var currentBrandRevisionVersioning = await UnitOfWork.BrandRepository.GetLatestRevisionVersionAsync(currentBrand.Id);

    if (currentBrandRevisionVersioning == null)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Revision version for Brand ID '{id}' does not exist");

    var currentBrandId = currentBrandRevisionVersioning.Id; // this will be parentRevisionId for the new revision

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = UnitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createBrandRevisionDto.Comment, userId, currentBrandId);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(500, $"Unable to create Entity Revision Metadata for Brand ID '{id}'.");

    var brandRevision = UnitOfWork.BrandRepository.CreateRevision(id, revisionMetadata, createBrandRevisionDto);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(500, $"Could not create brand revision for Brand ID '{id}'");

    var brandRevisionSnapshotDto = new BrandRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = brandRevision.Id,
      BrandId = brandRevision.BrandId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      ParentRevisionId = revisionMetadata.ParentRevisionId,
      Name = brandRevision.Name,
      Description = brandRevision.Description,
      Country = brandRevision.Country == null ? null : new CountryExerptDto
      {
        Id = brandRevision.Country.Id,
        Name = brandRevision.Country.Name
      }
    };

    return ServiceResult<BrandRevisionSnapshotDto>.Success(200, brandRevisionSnapshotDto);
  }

  public override async Task<ServiceResult<BrandRevisionSnapshotDto>> UpdateRevisionAsync(int revisionId, BrandCreateRevisionDto createBrandRevisionDto, ClaimsPrincipal userClaims)
  {
    var userId = UserClaimsUtils.GetUserId(userClaims);
    var revisionSnapshot = await UnitOfWork.BrandRepository.GetRevisionSnapshotAsync(revisionId, false, userId);
    if (revisionSnapshot == null)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' does not exist");

    if (revisionSnapshot.Status != RevisionStatus.Pending.ToString() && revisionSnapshot.Status != RevisionStatus.Draft.ToString())
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' is not pending or draft");

    if (revisionSnapshot.CreatedById != userId)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(403, $"User ID {userId} is not the author of Revision ID '{revisionId}'");

    BrandDto? currentBrand = null;
    var brandId = revisionSnapshot.BrandId;
    if (brandId != null)
    {
      currentBrand = await UnitOfWork.BrandRepository.GetByIdAsync(brandId.Value);
      if (currentBrand == null)
        return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Brand ID '{brandId}' does not exist");
    }

    if (!string.IsNullOrWhiteSpace(createBrandRevisionDto.Name))
    {
      var brandExistsParams = new BrandExistsParams
      {
        Name = createBrandRevisionDto.Name,
        ExcludeId = brandId
      };
      if (await UnitOfWork.BrandRepository.ExistsAsync(brandExistsParams))
      {
        return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Brand '{createBrandRevisionDto.Name}' already exists.");
      }
    }

    // Should make a revision, not update brand.
    // CreateBrandRevisionAsync will be used when revision is approved and committed
    // Since we're making BrandRevision AND RevisionEntity, unit of work would help here

    bool hasChangesCurrentBrandToRevisionDto = currentBrand == null ||
      currentBrand.Name != createBrandRevisionDto.Name ||
      currentBrand.Country?.Id != createBrandRevisionDto.CountryId ||
      currentBrand.Description != createBrandRevisionDto.Description;

    if (!hasChangesCurrentBrandToRevisionDto)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"No changes detected for Brand ID '{brandId}'. Revision not updated.");

    bool hasChangesCurrentRevisionToRevisionDto =
      revisionSnapshot.Name != createBrandRevisionDto.Name ||
      revisionSnapshot.Country?.Id != createBrandRevisionDto.CountryId ||
      revisionSnapshot.Description != createBrandRevisionDto.Description;

    if (!hasChangesCurrentRevisionToRevisionDto)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"No changes detected for Revision ID '{revisionId}'. Revision not updated.");

    var brandRevision = await UnitOfWork.BrandRepository.UpdateRevisionAsync(revisionId, createBrandRevisionDto);
    if (brandRevision == null)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Brand Revision ID '{revisionId}' does not exist.");

    var revisionMetadata = await UnitOfWork.RevisionMetadataRepository.UpdateRevisionMetadataAsync(revisionId, createBrandRevisionDto.Comment, userId);
    if (revisionMetadata == null)
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Revision Metadata ID '{revisionId}' does not exist.");

    // TODO: after implementing country
    // if (brandRevision.CountryId != null && !await UnitOfWork.CountryRepository.ExistsByIdAsync(brandRevision.CountryId))
    //   return ServiceResult<BrandRevisionSnapshotDto>.Failure(400, $"Country ID {brandRevision.CountryId} does not exist.");

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<BrandRevisionSnapshotDto>.Failure(500, $"Could not update Brand Revision '{revisionId}'.");

    var brandRevisionSnapshotDto = new BrandRevisionSnapshotDto
    {
      Id = brandRevision.Id,
      BrandId = brandRevision.BrandId,
      Comment = brandRevision.RevisionMetadata.Comment,
      Version = brandRevision.RevisionMetadata.Version,
      Status = brandRevision.RevisionMetadata.Status.ToString(),
      ParentRevisionId = brandRevision.RevisionMetadata.ParentRevisionId,
      Name = brandRevision.Name,
      Description = brandRevision.Description,
      Country = brandRevision.Country == null ? null : new CountryExerptDto
      {
        Id = brandRevision.Country.Id,
        Name = brandRevision.Country.Name
      },
      CreatedAt = brandRevision.RevisionMetadata.CreatedAt,
      CreatedBy = brandRevision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = brandRevision.RevisionMetadata.CreatedBy?.Id,
      UpdatedAt = brandRevision.RevisionMetadata.UpdatedAt,
      UpdatedBy = brandRevision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = brandRevision.RevisionMetadata.UpdatedBy?.Id,
    };

    return ServiceResult<BrandRevisionSnapshotDto>.Success(200, brandRevisionSnapshotDto);
  }

  public override async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, int brandId, HttpResponse response, ClaimsPrincipal user)
  {
    var brand = await UnitOfWork.BrandRepository.GetByIdAsync(brandId);

    if (brand == null)
      return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Failure(400, $"Brand ID '{brandId}' does not exist");

    var userIsModerator = user?.IsInRole("Moderator") == true;
    var brandRevisions = await UnitOfWork.BrandRepository.GetRevisionExcerptsAsync(revisionExcerptParams, brandId, userIsModerator);
    response.AddPaginationHeader(brandRevisions);
    response.Headers.Append("Brand-Name", brand.Name);
    response.Headers.AccessControlExposeHeaders = "Pagination, Brand-Name";

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, brandRevisions);
  }

  protected override RevisionDiffDto CalculateDiff(BrandRevisionSnapshotDto oldRevision, BrandRevisionSnapshotDto newRevision)
  {
    // if the brand is initial version, then it wont have an ID.
    // but since brand is initial version, there wont be two revisions of it to compare to.
    // we should never be able to get to this point if theres only an initial brand revision,
    // so I'm suppressing the nullable warning 
    var diff = InitializeRevisionDiff(oldRevision.BrandId!.Value, oldRevision, newRevision);

    // Compare each property and only add if different
    if (oldRevision.Name != newRevision.Name)
      diff.Changes["name"] = new Change { Old = oldRevision.Name, New = newRevision.Name };

    if (oldRevision.Description != newRevision.Description)
      diff.Changes["description"] = new Change { Old = oldRevision.Description, New = newRevision.Description };

    if (oldRevision.Country?.Id != newRevision.Country?.Id)
      diff.Changes["country"] = new Change
      {
        Old = oldRevision.Country == null ? null : new CountryExerptDto
        {
          Id = oldRevision.Country.Id,
          Name = oldRevision.Country.Name
        },

        New = newRevision.Country == null ? null : new CountryExerptDto
        {
          Id = newRevision.Country.Id,
          Name = newRevision.Country.Name
        }
      };

    return diff;
  }
}