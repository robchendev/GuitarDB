using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Enums;
using Backend.Extensions;
using Backend.Interfaces;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class LuthierService(IUnitOfWork unitOfWork) : ILuthierService
{
  public async Task<PagedList<LuthierDto>> GetLuthiersAsync(LuthierParams luthierParams, HttpResponse response)
  {
    var luthiers = await unitOfWork.LuthierRepository.GetLuthiersAsync(luthierParams);
    response.AddPaginationHeader(luthiers);

    return luthiers;
  }

  public async Task<ServiceResult<LuthierDto>> GetLuthierAsync(int id)
  {
    var luthier = await unitOfWork.LuthierRepository.GetLuthierByIdAsync(id);

    if (luthier == null)
      return ServiceResult<LuthierDto>.Failure(404, $"Luthier with ID {id} not found");

    return ServiceResult<LuthierDto>.Success(200, luthier);
  }

  public async Task<ServiceResult<LuthierRevisionSnapshotDto>> CreateInitialLuthierRevisionAsync(CreateLuthierRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims)
  {
    if (string.IsNullOrWhiteSpace(createLuthierRevisionDto.Name))
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, "Name must be provided");

    if (await unitOfWork.LuthierRepository.LuthierExistsAsync(createLuthierRevisionDto.Name))
    {
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier '{createLuthierRevisionDto.Name}' already exists");
    }

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = unitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createLuthierRevisionDto.Comment, userId, null);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(500, "Unable to create Entity Revision Metadata for initial luthier.");

    var luthierRevision = unitOfWork.LuthierRepository.CreateInitialLuthierRevisionAsync(revisionMetadata, createLuthierRevisionDto);

    if (!await unitOfWork.SaveAllAsync())
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

  public async Task<ServiceResult<LuthierRevisionSnapshotDto>> CreateLuthierRevisionAsync(int id, CreateLuthierRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims)
  {
    var currentLuthier = await unitOfWork.LuthierRepository.GetLuthierByIdAsync(id);

    if (currentLuthier == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier ID '{id}' does not exist");

    if (!string.IsNullOrWhiteSpace(createLuthierRevisionDto.Name))
    {
      if (await unitOfWork.LuthierRepository.LuthierExistsAsync(createLuthierRevisionDto.Name, id))
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

    var currentLuthierRevisionVersioning = await unitOfWork.LuthierRepository.GetLatestLuthierRevisionVersionAsync(currentLuthier.Id);

    if (currentLuthierRevisionVersioning == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Revision version for Luthier ID '{id}' does not exist");

    var currentLuthierId = currentLuthierRevisionVersioning.Id; // this will be parentRevisionId for the new revision

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = unitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createLuthierRevisionDto.Comment, userId, currentLuthierId);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(500, $"Unable to create Entity Revision Metadata for Luthier ID '{id}'.");

    var luthierRevision = unitOfWork.LuthierRepository.CreateLuthierRevisionAsync(id, revisionMetadata, createLuthierRevisionDto);

    if (!await unitOfWork.SaveAllAsync())
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

  public async Task<ServiceResult<LuthierRevisionSnapshotDto>> UpdateLuthierRevisionAsync(int revisionId, CreateLuthierRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims)
  {
    var userId = UserClaimsUtils.GetUserId(userClaims);
    var revisionSnapshot = await unitOfWork.LuthierRepository.GetLuthierRevisionSnapshotAsync(revisionId, false, userId);
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
      currentLuthier = await unitOfWork.LuthierRepository.GetLuthierByIdAsync(luthierId.Value);
      if (currentLuthier == null)
        return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier ID '{luthierId}' does not exist");
    }

    if (!string.IsNullOrWhiteSpace(createLuthierRevisionDto.Name))
    {
      if (await unitOfWork.LuthierRepository.LuthierExistsAsync(createLuthierRevisionDto.Name, luthierId))
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

    var luthierRevision = await unitOfWork.LuthierRepository.UpdateLuthierRevisionAsync(revisionId, createLuthierRevisionDto);
    if (luthierRevision == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Luthier Revision ID '{revisionId}' does not exist.");

    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.UpdateRevisionMetadataAsync(revisionId, createLuthierRevisionDto.Comment, userId);
    if (revisionMetadata == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(400, $"Revision Metadata ID '{revisionId}' does not exist.");

    if (!await unitOfWork.SaveAllAsync())
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

  public async Task<ServiceResult<object>> DeleteLuthierAsync(int id)
  {
    var luthierExists = await unitOfWork.LuthierRepository.DeleteLuthierAsync(id);
    if (!luthierExists)
      return ServiceResult<object>.Failure(400, $"Luthier ID '{id}' does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<object>.Failure(500, "Could not delete luthier (other entities reference luthier)");

    return ServiceResult<object>.Success(200, null);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetLuthierRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, int luthierId, HttpResponse response, ClaimsPrincipal user)
  {
    var luthier = await unitOfWork.LuthierRepository.GetLuthierByIdAsync(luthierId);

    if (luthier == null)
      return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Failure(400, $"Luthier ID '{luthierId}' does not exist");

    var userIsModerator = user?.IsInRole("Moderator") == true;
    var luthierRevisions = await unitOfWork.LuthierRepository.GetLuthierRevisionExcerptsAsync(revisionExcerptParams, luthierId, userIsModerator);
    response.AddPaginationHeader(luthierRevisions);
    response.Headers.Append("Luthier-Name", luthier.Name);
    response.Headers.AccessControlExposeHeaders = "Pagination, Luthier-Name";

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, luthierRevisions);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewLuthierRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, HttpResponse response)
  {
    var luthierRevisions = await unitOfWork.LuthierRepository.GetNewLuthierRevisionExcerptsAsync(revisionExcerptParams);
    response.AddPaginationHeader(luthierRevisions);

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, luthierRevisions);
  }

  public async Task<ServiceResult<LuthierRevisionSnapshotDto>> GetLuthierRevisionSnapshotAsync(int revisionId, ClaimsPrincipal user)
  {
    var userIsModerator = user.IsInRole("Moderator") == true;
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var luthierRevision = await unitOfWork.LuthierRepository.GetLuthierRevisionSnapshotAsync(revisionId, userIsModerator, userId);

    if (luthierRevision == null)
      return ServiceResult<LuthierRevisionSnapshotDto>.Failure(404, $"Luthier Revision ID '{revisionId}' not found or user has no access");

    return ServiceResult<LuthierRevisionSnapshotDto>.Success(200, luthierRevision);
  }

  public async Task<ServiceResult<RevisionDiffDto>> GetLuthierRevisionDiffAsync(int revisionId1, int revisionId2, int luthierId, ClaimsPrincipal user)
  {
    if (revisionId1 == revisionId2)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Luthier Revision ID '{revisionId1}' cannot be compared with itself");

    var userIsModerator = user.IsInRole("Moderator") == true;
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var luthierRevision1 = await unitOfWork.LuthierRepository.GetLuthierRevisionSnapshotAsync(revisionId1, userIsModerator, userId);
    if (luthierRevision1 == null)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Luthier Revision ID '{revisionId1}' not found or user has no access.");

    var luthierRevision2 = await unitOfWork.LuthierRepository.GetLuthierRevisionSnapshotAsync(revisionId2, userIsModerator, userId);
    if (luthierRevision2 == null)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Luthier Revision ID '{revisionId2}' not found or user has no access.");

    // must check if the revisions are actually for the same luthier
    if (luthierRevision1.LuthierId != luthierRevision2.LuthierId)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Luthier Revision IDs {luthierRevision1.Id} and {luthierRevision2.Id} belong to different luthiers");

    bool rev1Older = luthierRevision1.CreatedAt <= luthierRevision2.CreatedAt;
    var diff = CalculateDiff(rev1Older ? luthierRevision1 : luthierRevision2, rev1Older ? luthierRevision2 : luthierRevision1);

    return ServiceResult<RevisionDiffDto>.Success(200, diff);
  }

  private static RevisionDiffDto CalculateDiff(LuthierRevisionSnapshotDto oldRevision, LuthierRevisionSnapshotDto newRevision)
  {
    // if the luthier is initial version, then it wont have an ID.
    // but since luthier is initial version, there wont be two revisions of it to compare to.
    // we should never be able to get to this point if theres only an initial luthier revision,
    // so I'm suppressing the nullable warning 
    var diff = new RevisionDiffDto { EntityId = oldRevision.LuthierId!.Value };

    // Add basic properties
    diff.Changes["id"] = new Change { Old = oldRevision.Id, New = newRevision.Id };
    diff.Changes["createdAt"] = new Change { Old = oldRevision.CreatedAt, New = newRevision.CreatedAt };
    diff.Changes["createdBy"] = new Change { Old = oldRevision.CreatedBy, New = newRevision.CreatedBy };
    diff.Changes["version"] = new Change { Old = oldRevision.Version, New = newRevision.Version };
    diff.Changes["comment"] = new Change { Old = oldRevision.Comment, New = newRevision.Comment };

    // Compare each property and only add if different
    if (oldRevision.Name != newRevision.Name)
      diff.Changes["name"] = new Change { Old = oldRevision.Name, New = newRevision.Name };

    if (oldRevision.Description != newRevision.Description)
      diff.Changes["description"] = new Change { Old = oldRevision.Description, New = newRevision.Description };

    diff.Iteration = newRevision.Iteration;

    return diff;
  }

  // the reason approve needs to be here is because if we only have revisionId, we dont know which DB table to query
  // is the revision in LuthierRevisions? BrewerRevisions? Idk! 
  // I can get the RevisionMetadata, but that tells me nothing about the EntityType (Luthier, Grinder, etc)

  public async Task<ServiceResult<LuthierDto>> ApproveCreateLuthierRevisionAsync(int revisionId, int iteration, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(revisionId);
    if (revisionMetadata == null)
      return ServiceResult<LuthierDto>.Failure(400, $"Entity Revision ID '{revisionId}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<LuthierDto>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    if (revisionMetadata.Iteration != iteration)
      return ServiceResult<LuthierDto>.Failure(409, $"Cannot approve Entity Revision ID '{revisionId}' because it has changed since the approved review was loaded.");

    var parentRevisionId = revisionMetadata.ParentRevisionId;
    if (parentRevisionId != null)
      return ServiceResult<LuthierDto>.Failure(400, $"Entity Revision cannot have a Parent Revision ID when creating a new Luthier. Current Parent Revision ID: {parentRevisionId}");

    var luthierRevision = await unitOfWork.LuthierRepository.GetLuthierRevisionEntityAsync(revisionId);
    if (luthierRevision == null)
      return ServiceResult<LuthierDto>.Failure(400, $"Luthier Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (luthierRevision.LuthierId != null)
      return ServiceResult<LuthierDto>.Failure(400, $"Cannot create Luthier because Luthier Revision with ID '{revisionId}' is tied to an existing Luthier with ID '{luthierRevision.LuthierId}'.");

    // TODO: Ensure luthier's name+brand or name+luthier is unique
    if (await unitOfWork.LuthierRepository.LuthierExistsAsync(luthierRevision.Name))
      return ServiceResult<LuthierDto>.Failure(400, $"Luthier '{luthierRevision.Name}' already exists");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to the DB

    var luthier = unitOfWork.LuthierRepository.CreateLuthier(luthierRevision);

    var approvalResult = await unitOfWork.RevisionMetadataRepository.ApprovePendingRevisionMetadataAsync(luthierRevision.Id, userId, null);
    if (!approvalResult)
      return ServiceResult<LuthierDto>.Failure(400, $"Revision Metadata with ID {luthierRevision.Id} does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<LuthierDto>.Failure(500, $"Could not create Luthier using Luthier Revision ID {luthierRevision.Id}.");

    var luthierDto = new LuthierDto
    {
      Id = luthier.Id,
      Name = luthierRevision.Name,
      Description = luthierRevision.Description,
    };

    return ServiceResult<LuthierDto>.Success(200, luthierDto);
  }

  public async Task<ServiceResult<LuthierDto>> ApproveUpdateLuthierRevisionAsync(int luthierId, int revisionId, int iteration, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(revisionId);
    if (revisionMetadata == null)
      return ServiceResult<LuthierDto>.Failure(400, $"Entity Revision ID '{revisionId}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<LuthierDto>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    if (revisionMetadata.Iteration != iteration)
      return ServiceResult<LuthierDto>.Failure(409, $"Cannot approve Entity Revision ID '{revisionId}' because it has changed since the approved review was loaded.");

    var oldParentRevisionId = revisionMetadata.ParentRevisionId;
    if (oldParentRevisionId == null)
      return ServiceResult<LuthierDto>.Failure(400, $"Entity Revision must have a Parent Revision ID when updating an existing Luthier.");

    var luthierRevisionEntity = await unitOfWork.LuthierRepository.GetLuthierRevisionEntityAsync(revisionId);
    if (luthierRevisionEntity == null)
      return ServiceResult<LuthierDto>.Failure(400, $"Luthier Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (luthierRevisionEntity.LuthierId != luthierId)
      return ServiceResult<LuthierDto>.Failure(400, $"Luthier ID {(luthierRevisionEntity.LuthierId == null ? "null" : luthierRevisionEntity.LuthierId)} on Luthier Revision and provided Luthier ID {luthierId} are different.");

    var currentLuthierRevisionVersioning = await unitOfWork.LuthierRepository.GetLatestLuthierRevisionVersionAsync(luthierId);
    if (currentLuthierRevisionVersioning == null)
      return ServiceResult<LuthierDto>.Failure(400, $"Revision version for Luthier ID '{luthierId}' does not exist");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to DB

    var luthier = await unitOfWork.LuthierRepository.UpdateLuthierAsync(luthierRevisionEntity);

    if (luthier == null)
      return ServiceResult<LuthierDto>.Failure(400, $"Luthier with ID {luthierId} does not exist");

    var approvalResult = await unitOfWork.RevisionMetadataRepository.ApprovePendingRevisionMetadataAsync(revisionId, userId, currentLuthierRevisionVersioning.Version);
    if (!approvalResult)
      return ServiceResult<LuthierDto>.Failure(400, $"Revision Metadata with ID {luthierRevisionEntity.Id} does not exist, or Version number {currentLuthierRevisionVersioning.Version} is not 1 or greater.");

    if (oldParentRevisionId != null)
    {
      await unitOfWork.RevisionMetadataRepository.AdoptPendingRevisionMetadatasAsync(oldParentRevisionId.Value, revisionId, userId);
    }

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<LuthierDto>.Failure(500, $"Could not update Luthier with ID {luthierId} for luthier revision with ID {luthierRevisionEntity.Id}");

    var luthierDto = new LuthierDto
    {
      Id = luthier.Id,
      Name = luthierRevisionEntity.Name,
      Description = luthierRevisionEntity.Description,
    };

    return ServiceResult<LuthierDto>.Success(200, luthierDto);
  }
}
