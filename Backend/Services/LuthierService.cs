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

public class LuthierService : RevisableEntityService<
  Luthier,
  LuthierDto,
  LuthierSearchParams,
  LuthierExistsParams,
  LuthierRevision,
  LuthierRevisionSnapshotDto,
  LuthierCreateRevisionDto
>, ILuthierService
{
  public LuthierService(IUnitOfWork unitOfWork) : base(unitOfWork, "Luthier") { }

  protected override IRevisableEntityRepository<
    Luthier,
    LuthierDto,
    LuthierSearchParams,
    LuthierExistsParams,
    LuthierRevision,
    LuthierRevisionSnapshotDto,
    LuthierCreateRevisionDto
  >
    GetRepository() => UnitOfWork.LuthierRepository;

  protected override int? GetEntityId(LuthierRevision revisionEntity) => revisionEntity.LuthierId;

  protected override string GetEntityName(LuthierDto entity) => entity.Name;

  public override async Task<ServiceResult<LuthierRevisionSnapshotDto>> CreateInitialRevisionAsync(LuthierCreateRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims)
  {
    if (string.IsNullOrWhiteSpace(createLuthierRevisionDto.Name))
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, "Name must be provided");

    var luthierExistsParams = new LuthierExistsParams
    {
      Name = createLuthierRevisionDto.Name
      // TODO add CountryId
    };
    if (await UnitOfWork.LuthierRepository.ExistsAsync(luthierExistsParams))
    {
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier '{createLuthierRevisionDto.Name}' already exists");
    }

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = UnitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createLuthierRevisionDto.Comment, userId, null);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(500, "Unable to create Entity Revision Metadata for initial luthier.");

    var luthierRevision = UnitOfWork.LuthierRepository.CreateInitialRevision(revisionMetadata, createLuthierRevisionDto);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(500, "Could not create initial luthier revision");

    var luthierRevisionSnapshotDto = new LuthierRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = luthierRevision.Id,
      LuthierId = luthierRevision.LuthierId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      Name = luthierRevision.Name,
      Description = luthierRevision.Description,
    };

    return ServiceResult<LuthierRevisionSnapshotDto>.Success(200, luthierRevisionSnapshotDto);
  }

  public override async Task<ServiceResult<LuthierRevisionSnapshotDto>> CreateRevisionAsync(int id, LuthierCreateRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims)
  {
    var currentLuthier = await UnitOfWork.LuthierRepository.GetByIdAsync(id);

    if (currentLuthier == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier ID '{id}' does not exist");

    if (!string.IsNullOrWhiteSpace(createLuthierRevisionDto.Name))
    {
      var luthierExistsParams = new LuthierExistsParams
      {
        Name = createLuthierRevisionDto.Name,
        ExcludeId = id
        // TODO add CountryId
      };
      if (await UnitOfWork.LuthierRepository.ExistsAsync(luthierExistsParams))
      {
        return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier '{createLuthierRevisionDto.Name}' already exists.");
      }
    }

    // Should make a revision, not update luthier.
    // CreateLuthierRevisionAsync will be used when revision is approved and committed
    // Since we're making LuthierRevision AND RevisionEntity, unit of work would help here

    bool hasChanges =
      currentLuthier.Name != createLuthierRevisionDto.Name ||
      currentLuthier.Description != createLuthierRevisionDto.Description;

    if (!hasChanges)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"No changes detected for Luthier ID '{id}'. Revision not created.");

    var currentLuthierRevisionVersioning = await UnitOfWork.LuthierRepository.GetLatestRevisionVersionAsync(currentLuthier.Id);
    if (currentLuthierRevisionVersioning == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Revision version for Luthier ID '{id}' does not exist");

    var currentLuthierId = currentLuthierRevisionVersioning.Id; // this will be parentRevisionId for the new revision

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = UnitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createLuthierRevisionDto.Comment, userId, currentLuthierId);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(500, $"Unable to create Entity Revision Metadata for Luthier ID '{id}'.");

    var luthierRevision = UnitOfWork.LuthierRepository.CreateRevision(id, revisionMetadata, createLuthierRevisionDto);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(500, $"Could not create luthier revision for Luthier ID '{id}'");

    var luthierRevisionSnapshotDto = new LuthierRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = luthierRevision.Id,
      LuthierId = luthierRevision.LuthierId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      ParentRevisionId = revisionMetadata.ParentRevisionId,
      Name = luthierRevision.Name,
      Description = luthierRevision.Description,
    };

    return ServiceResult<LuthierRevisionSnapshotDto>.Success(200, luthierRevisionSnapshotDto);
  }

  public override async Task<ServiceResult<LuthierRevisionSnapshotDto>> UpdateRevisionAsync(int revisionId, LuthierCreateRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims)
  {
    var userId = UserClaimsUtils.GetUserId(userClaims);
    var revisionSnapshot = await UnitOfWork.LuthierRepository.GetRevisionSnapshotAsync(revisionId, false, userId);
    if (revisionSnapshot == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' does not exist");

    if (revisionSnapshot.Status != RevisionStatus.Pending.ToString() && revisionSnapshot.Status != RevisionStatus.Draft.ToString())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' is not pending or draft");

    if (revisionSnapshot.CreatedById != userId)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(403, $"User ID {userId} is not the author of Revision ID '{revisionId}'");

    LuthierDto? currentLuthier = null;
    var luthierId = revisionSnapshot.LuthierId;
    if (luthierId != null)
    {
      currentLuthier = await UnitOfWork.LuthierRepository.GetByIdAsync(luthierId.Value);
      if (currentLuthier == null)
        return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier ID '{luthierId}' does not exist");
    }

    if (!string.IsNullOrWhiteSpace(createLuthierRevisionDto.Name))
    {
      var luthierExistsParams = new LuthierExistsParams
      {
        Name = createLuthierRevisionDto.Name,
        ExcludeId = luthierId
        // TODO add CountryId
      };
      if (await UnitOfWork.LuthierRepository.ExistsAsync(luthierExistsParams))
      {
        return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier '{createLuthierRevisionDto.Name}' already exists.");
      }
    }

    // Should make a revision, not update luthier.
    // CreateLuthierRevisionAsync will be used when revision is approved and committed
    // Since we're making LuthierRevision AND RevisionEntity, unit of work would help here

    bool hasChangesCurrentLuthierToRevisionDto = currentLuthier == null ||
      currentLuthier.Name != createLuthierRevisionDto.Name ||
      currentLuthier.Description != createLuthierRevisionDto.Description;

    if (!hasChangesCurrentLuthierToRevisionDto)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"No changes detected for Luthier ID '{luthierId}'. Revision not updated.");

    bool hasChangesCurrentRevisionToRevisionDto =
      revisionSnapshot.Name != createLuthierRevisionDto.Name ||
      revisionSnapshot.Description != createLuthierRevisionDto.Description;

    if (!hasChangesCurrentRevisionToRevisionDto)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"No changes detected for Revision ID '{revisionId}'. Revision not updated.");

    var luthierRevision = await UnitOfWork.LuthierRepository.UpdateRevisionAsync(revisionId, createLuthierRevisionDto);
    if (luthierRevision == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier Revision ID '{revisionId}' does not exist.");

    var revisionMetadata = await UnitOfWork.RevisionMetadataRepository.UpdateRevisionMetadataAsync(revisionId, createLuthierRevisionDto.Comment, userId);
    if (revisionMetadata == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Revision Metadata ID '{revisionId}' does not exist.");

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(500, $"Could not update Luthier Revision '{revisionId}'.");

    var luthierRevisionSnapshotDto = new LuthierRevisionSnapshotDto
    {
      Id = luthierRevision.Id,
      LuthierId = luthierRevision.LuthierId,
      Comment = luthierRevision.RevisionMetadata.Comment,
      Version = luthierRevision.RevisionMetadata.Version,
      Status = luthierRevision.RevisionMetadata.Status.ToString(),
      ParentRevisionId = luthierRevision.RevisionMetadata.ParentRevisionId,
      Name = luthierRevision.Name,
      Description = luthierRevision.Description,
      CreatedAt = luthierRevision.RevisionMetadata.CreatedAt,
      CreatedBy = luthierRevision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = luthierRevision.RevisionMetadata.CreatedBy?.Id,
      UpdatedAt = luthierRevision.RevisionMetadata.UpdatedAt,
      UpdatedBy = luthierRevision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = luthierRevision.RevisionMetadata.UpdatedBy?.Id,
    };

    return ServiceResult<LuthierRevisionSnapshotDto>.Success(200, luthierRevisionSnapshotDto);
  }

  public override async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, int luthierId, HttpResponse response, ClaimsPrincipal user)
  {
    var luthier = await UnitOfWork.LuthierRepository.GetByIdAsync(luthierId);

    if (luthier == null)
      return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Failure(400, $"Luthier ID '{luthierId}' does not exist");

    var userIsModerator = user?.IsInRole("Moderator") == true;
    var luthierRevisions = await UnitOfWork.LuthierRepository.GetRevisionExcerptsAsync(revisionExcerptParams, luthierId, userIsModerator);
    response.AddPaginationHeader(luthierRevisions);
    response.Headers.Append("Luthier-Name", luthier.Name);
    response.Headers.AccessControlExposeHeaders = "Pagination, Luthier-Name";

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, luthierRevisions);
  }

  protected override RevisionDiffDto CalculateDiff(LuthierRevisionSnapshotDto oldRevision, LuthierRevisionSnapshotDto newRevision)
  {
    // if the luthier is initial version, then it wont have an ID.
    // but since luthier is initial version, there wont be two revisions of it to compare to.
    // we should never be able to get to this point if theres only an initial luthier revision,
    // so I'm suppressing the nullable warning 
    var diff = InitializeRevisionDiff(oldRevision.LuthierId!.Value, oldRevision, newRevision);

    // Compare each property and only add if different
    if (oldRevision.Name != newRevision.Name)
      diff.Changes["name"] = new Change { Old = oldRevision.Name, New = newRevision.Name };

    if (oldRevision.Description != newRevision.Description)
      diff.Changes["description"] = new Change { Old = oldRevision.Description, New = newRevision.Description };

    return diff;
  }
}