using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Enums;
using Backend.Extensions;
using Backend.Interfaces;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class GuitarService(IUnitOfWork unitOfWork) : IGuitarService
{
  public async Task<PagedList<GuitarDto>> GetGuitarsAsync(GuitarParams guitarParams, HttpResponse response)
  {
    var guitars = await unitOfWork.GuitarRepository.GetGuitarsAsync(guitarParams);
    response.AddPaginationHeader(guitars);

    return guitars;
  }

  public async Task<ServiceResult<GuitarDto>> GetGuitarAsync(int id)
  {
    var guitar = await unitOfWork.GuitarRepository.GetGuitarByIdAsync(id);

    if (guitar == null)
      return ServiceResult<GuitarDto>.Failure(404, $"Guitar with ID {id} not found");

    return ServiceResult<GuitarDto>.Success(200, guitar);
  }

  public async Task<ServiceResult<GuitarRevisionSnapshotDto>> CreateInitialGuitarRevisionAsync(CreateGuitarRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims)
  {
    if (string.IsNullOrWhiteSpace(createGuitarRevisionDto.Name))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, "Name must be provided");

    if (await unitOfWork.GuitarRepository.GuitarExistsAsync(createGuitarRevisionDto.Name))
    {
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar '{createGuitarRevisionDto.Name}' already exists");
    }

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = unitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createGuitarRevisionDto.Comment, userId, null);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(500, "Unable to create Entity Revision Metadata for initial guitar.");

    var guitarRevision = unitOfWork.GuitarRepository.CreateInitialGuitarRevisionAsync(revisionMetadata, createGuitarRevisionDto);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(500, "Could not create initial guitar revision");

    var guitarRevisionSnapshotDto = new GuitarRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = guitarRevision.Id,
      GuitarId = guitarRevision.GuitarId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      Name = guitarRevision.Name,
      Description = guitarRevision.Description,
      Luthier = guitarRevision.Luthier == null ? null : new LuthierExerptDto
      {
        Id = guitarRevision.Luthier.Id,
        Name = guitarRevision.Luthier.Name
      }
    };

    return ServiceResult<GuitarRevisionSnapshotDto>.Success(200, guitarRevisionSnapshotDto);
  }

  public async Task<ServiceResult<GuitarRevisionSnapshotDto>> CreateGuitarRevisionAsync(int id, CreateGuitarRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims)
  {
    var currentGuitar = await unitOfWork.GuitarRepository.GetGuitarByIdAsync(id);

    if (currentGuitar == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar ID '{id}' does not exist");

    if (!string.IsNullOrWhiteSpace(createGuitarRevisionDto.Name) && await unitOfWork.GuitarRepository.GuitarExistsAsync(createGuitarRevisionDto.Name, null, id))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar '{createGuitarRevisionDto.Name}' already exists.");

    if (createGuitarRevisionDto.LuthierId != null && await unitOfWork.LuthierRepository.LuthierExistsByIdAsync(createGuitarRevisionDto.LuthierId.Value))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Luthier ID {createGuitarRevisionDto.LuthierId} does not exist.");

    // Should make a revision, not update guitar.
    // CreateGuitarRevisionAsync will be used when revision is approved and committed
    // Since we're making GuitarRevision AND RevisionEntity, unit of work would help here

    bool hasChanges =
      currentGuitar.Name != createGuitarRevisionDto.Name ||
      currentGuitar.Luthier?.Id != createGuitarRevisionDto.LuthierId ||
      currentGuitar.Description != createGuitarRevisionDto.Description;

    if (!hasChanges)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"No changes detected for Guitar ID '{id}'. Revision not created.");

    var currentGuitarRevisionVersioning = await unitOfWork.GuitarRepository.GetLatestGuitarRevisionVersionAsync(currentGuitar.Id);

    if (currentGuitarRevisionVersioning == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Revision version for Guitar ID '{id}' does not exist");

    var currentGuitarId = currentGuitarRevisionVersioning.Id; // this will be parentRevisionId for the new revision

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = unitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createGuitarRevisionDto.Comment, userId, currentGuitarId);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(500, $"Unable to create Entity Revision Metadata for Guitar ID '{id}'.");

    var guitarRevision = unitOfWork.GuitarRepository.CreateGuitarRevisionAsync(id, revisionMetadata, createGuitarRevisionDto);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(500, $"Could not create guitar revision for Guitar ID '{id}'");

    var guitarRevisionSnapshotDto = new GuitarRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = guitarRevision.Id,
      GuitarId = guitarRevision.GuitarId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      ParentRevisionId = revisionMetadata.ParentRevisionId,
      Name = guitarRevision.Name,
      Description = guitarRevision.Description,
      Luthier = guitarRevision.Luthier == null ? null : new LuthierExerptDto
      {
        Id = guitarRevision.Luthier.Id,
        Name = guitarRevision.Luthier.Name
      }
    };

    return ServiceResult<GuitarRevisionSnapshotDto>.Success(200, guitarRevisionSnapshotDto);
  }

  public async Task<ServiceResult<GuitarRevisionSnapshotDto>> UpdateGuitarRevisionAsync(int revisionId, CreateGuitarRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims)
  {
    var userId = UserClaimsUtils.GetUserId(userClaims);
    var revisionSnapshot = await unitOfWork.GuitarRepository.GetGuitarRevisionSnapshotAsync(revisionId, false, userId);
    if (revisionSnapshot == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' does not exist");

    if (revisionSnapshot.Status != RevisionStatus.Pending.ToString() && revisionSnapshot.Status != RevisionStatus.Draft.ToString())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' is not pending or draft");

    if (revisionSnapshot.CreatedById != userId)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(403, $"User ID {userId} is not the author of Revision ID '{revisionId}'");

    GuitarDto? currentGuitar = null;
    var guitarId = revisionSnapshot.GuitarId;
    if (guitarId != null)
    {
      currentGuitar = await unitOfWork.GuitarRepository.GetGuitarByIdAsync(guitarId.Value);
      if (currentGuitar == null)
        return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar ID '{guitarId}' does not exist");
    }

    if (!string.IsNullOrWhiteSpace(createGuitarRevisionDto.Name))
    {
      if (await unitOfWork.GuitarRepository.GuitarExistsAsync(createGuitarRevisionDto.Name, guitarId))
      {
        return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar '{createGuitarRevisionDto.Name}' already exists.");
      }
    }

    // Should make a revision, not update guitar.
    // CreateGuitarRevisionAsync will be used when revision is approved and committed
    // Since we're making GuitarRevision AND RevisionEntity, unit of work would help here

    bool hasChangesCurrentGuitarToRevisionDto = currentGuitar == null ||
      currentGuitar.Name != createGuitarRevisionDto.Name ||
      currentGuitar.Luthier?.Id != createGuitarRevisionDto.LuthierId ||
      currentGuitar.Description != createGuitarRevisionDto.Description;

    if (!hasChangesCurrentGuitarToRevisionDto)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"No changes detected for Guitar ID '{guitarId}'. Revision not updated.");

    bool hasChangesCurrentRevisionToRevisionDto =
      revisionSnapshot.Name != createGuitarRevisionDto.Name ||
      revisionSnapshot.Luthier?.Id != createGuitarRevisionDto.LuthierId ||
      revisionSnapshot.Description != createGuitarRevisionDto.Description;

    if (!hasChangesCurrentRevisionToRevisionDto)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"No changes detected for Revision ID '{revisionId}'. Revision not updated.");

    var guitarRevision = await unitOfWork.GuitarRepository.UpdateGuitarRevisionAsync(revisionId, createGuitarRevisionDto);
    if (guitarRevision == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar Revision ID '{revisionId}' does not exist.");

    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.UpdateRevisionMetadataAsync(revisionId, createGuitarRevisionDto.Comment, userId);
    if (revisionMetadata == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Revision Metadata ID '{revisionId}' does not exist.");

    if (guitarRevision.LuthierId != null && await unitOfWork.LuthierRepository.LuthierExistsByIdAsync(guitarRevision.LuthierId.Value))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Luthier ID {guitarRevision.LuthierId} does not exist.");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(500, $"Could not update Guitar Revision '{revisionId}'.");

    var guitarRevisionSnapshotDto = new GuitarRevisionSnapshotDto
    {
      Id = guitarRevision.Id,
      GuitarId = guitarRevision.GuitarId,
      Comment = guitarRevision.RevisionMetadata.Comment,
      Version = guitarRevision.RevisionMetadata.Version,
      Status = guitarRevision.RevisionMetadata.Status.ToString(),
      ParentRevisionId = guitarRevision.RevisionMetadata.ParentRevisionId,
      Name = guitarRevision.Name,
      Description = guitarRevision.Description,
      Luthier = guitarRevision.Luthier == null ? null : new LuthierExerptDto
      {
        Id = guitarRevision.Luthier.Id,
        Name = guitarRevision.Luthier.Name
      },
      CreatedAt = guitarRevision.RevisionMetadata.CreatedAt,
      CreatedBy = guitarRevision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = guitarRevision.RevisionMetadata.CreatedBy?.Id,
      UpdatedAt = guitarRevision.RevisionMetadata.UpdatedAt,
      UpdatedBy = guitarRevision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = guitarRevision.RevisionMetadata.UpdatedBy?.Id,
    };

    return ServiceResult<GuitarRevisionSnapshotDto>.Success(200, guitarRevisionSnapshotDto);
  }

  public async Task<ServiceResult<object>> DeleteGuitarAsync(int id)
  {
    var guitarExists = await unitOfWork.GuitarRepository.DeleteGuitarAsync(id);
    if (!guitarExists)
      return ServiceResult<object>.Failure(400, $"Guitar ID '{id}' does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<object>.Failure(500, "Could not delete guitar (other entities reference guitar)");

    return ServiceResult<object>.Success(200, null);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetGuitarRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, int guitarId, HttpResponse response, ClaimsPrincipal user)
  {
    var guitar = await unitOfWork.GuitarRepository.GetGuitarByIdAsync(guitarId);

    if (guitar == null)
      return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Failure(400, $"Guitar ID '{guitarId}' does not exist");

    var userIsModerator = user?.IsInRole("Moderator") == true;
    var guitarRevisions = await unitOfWork.GuitarRepository.GetGuitarRevisionExcerptsAsync(revisionExcerptParams, guitarId, userIsModerator);
    response.AddPaginationHeader(guitarRevisions);
    response.Headers.Append("Guitar-Name", guitar.Name);
    response.Headers.AccessControlExposeHeaders = "Pagination, Guitar-Name";

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, guitarRevisions);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewGuitarRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, HttpResponse response)
  {
    var guitarRevisions = await unitOfWork.GuitarRepository.GetNewGuitarRevisionExcerptsAsync(revisionExcerptParams);
    response.AddPaginationHeader(guitarRevisions);

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, guitarRevisions);
  }

  public async Task<ServiceResult<GuitarRevisionSnapshotDto>> GetGuitarRevisionSnapshotAsync(int revisionId, ClaimsPrincipal user)
  {
    var userIsModerator = user.IsInRole("Moderator") == true;
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var guitarRevision = await unitOfWork.GuitarRepository.GetGuitarRevisionSnapshotAsync(revisionId, userIsModerator, userId);

    if (guitarRevision == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(404, $"Guitar Revision ID '{revisionId}' not found or user has no access");

    return ServiceResult<GuitarRevisionSnapshotDto>.Success(200, guitarRevision);
  }

  public async Task<ServiceResult<RevisionDiffDto>> GetGuitarRevisionDiffAsync(int revisionId1, int revisionId2, int guitarId, ClaimsPrincipal user)
  {
    if (revisionId1 == revisionId2)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Guitar Revision ID '{revisionId1}' cannot be compared with itself");

    var userIsModerator = user.IsInRole("Moderator") == true;
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var guitarRevision1 = await unitOfWork.GuitarRepository.GetGuitarRevisionSnapshotAsync(revisionId1, userIsModerator, userId);
    if (guitarRevision1 == null)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Guitar Revision ID '{revisionId1}' not found or user has no access.");

    var guitarRevision2 = await unitOfWork.GuitarRepository.GetGuitarRevisionSnapshotAsync(revisionId2, userIsModerator, userId);
    if (guitarRevision2 == null)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Guitar Revision ID '{revisionId2}' not found or user has no access.");

    // must check if the revisions are actually for the same guitar
    if (guitarRevision1.GuitarId != guitarRevision2.GuitarId)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Guitar Revision IDs {guitarRevision1.Id} and {guitarRevision2.Id} belong to different guitars");

    bool rev1Older = guitarRevision1.CreatedAt <= guitarRevision2.CreatedAt;
    var diff = CalculateDiff(rev1Older ? guitarRevision1 : guitarRevision2, rev1Older ? guitarRevision2 : guitarRevision1);

    return ServiceResult<RevisionDiffDto>.Success(200, diff);
  }

  private static RevisionDiffDto CalculateDiff(GuitarRevisionSnapshotDto oldRevision, GuitarRevisionSnapshotDto newRevision)
  {
    // if the guitar is initial version, then it wont have an ID.
    // but since guitar is initial version, there wont be two revisions of it to compare to.
    // we should never be able to get to this point if theres only an initial guitar revision,
    // so I'm suppressing the nullable warning 
    var diff = new RevisionDiffDto { EntityId = oldRevision.GuitarId!.Value };

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

    if (oldRevision.Luthier?.Id != newRevision.Luthier?.Id)
      diff.Changes["luthier"] = new Change
      {
        Old = oldRevision.Luthier == null ? null : new LuthierExerptDto
        {
          Id = oldRevision.Luthier.Id,
          Name = oldRevision.Luthier.Name
        },

        New = newRevision.Luthier == null ? null : new LuthierExerptDto
        {
          Id = newRevision.Luthier.Id,
          Name = newRevision.Luthier.Name
        }
      };

    diff.Iteration = newRevision.Iteration;

    return diff;
  }

  // the reason approve needs to be here is because if we only have revisionId, we dont know which DB table to query
  // is the revision in GuitarRevisions? BrewerRevisions? Idk! 
  // I can get the RevisionMetadata, but that tells me nothing about the EntityType (Guitar, Grinder, etc)

  public async Task<ServiceResult<GuitarDto>> ApproveCreateGuitarRevisionAsync(int revisionId, int iteration, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(revisionId);
    if (revisionMetadata == null)
      return ServiceResult<GuitarDto>.Failure(400, $"Entity Revision ID '{revisionId}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<GuitarDto>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    if (revisionMetadata.Iteration != iteration)
      return ServiceResult<GuitarDto>.Failure(409, $"Cannot approve Entity Revision ID '{revisionId}' because it has changed since the approved review was loaded.");

    var parentRevisionId = revisionMetadata.ParentRevisionId;
    if (parentRevisionId != null)
      return ServiceResult<GuitarDto>.Failure(400, $"Entity Revision cannot have a Parent Revision ID when creating a new Guitar. Current Parent Revision ID: {parentRevisionId}");

    var guitarRevision = await unitOfWork.GuitarRepository.GetGuitarRevisionEntityAsync(revisionId);
    if (guitarRevision == null)
      return ServiceResult<GuitarDto>.Failure(400, $"Guitar Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (guitarRevision.GuitarId != null)
      return ServiceResult<GuitarDto>.Failure(400, $"Cannot create Guitar because Guitar Revision with ID '{revisionId}' is tied to an existing Guitar with ID '{guitarRevision.GuitarId}'.");

    // TODO: Ensure guitar's name+brand or name+luthier is unique
    if (await unitOfWork.GuitarRepository.GuitarExistsAsync(guitarRevision.Name))
    {
      return ServiceResult<GuitarDto>.Failure(400, $"Guitar '{guitarRevision.Name}' already exists");
    }

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to the DB

    var guitar = unitOfWork.GuitarRepository.CreateGuitar(guitarRevision);

    var approvalResult = await unitOfWork.RevisionMetadataRepository.ApprovePendingRevisionMetadataAsync(guitarRevision.Id, userId, null);
    if (!approvalResult)
      return ServiceResult<GuitarDto>.Failure(400, $"Revision Metadata with ID {guitarRevision.Id} does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<GuitarDto>.Failure(500, $"Could not create Guitar using Guitar Revision ID {guitarRevision.Id}.");

    var guitarDto = new GuitarDto
    {
      Id = guitar.Id,
      Name = guitarRevision.Name,
      Description = guitarRevision.Description,
      Luthier = guitarRevision.Luthier == null ? null : new LuthierExerptDto
      {
        Id = guitarRevision.Luthier.Id,
        Name = guitarRevision.Luthier.Name,
      }
    };

    return ServiceResult<GuitarDto>.Success(200, guitarDto);
  }

  public async Task<ServiceResult<GuitarDto>> ApproveUpdateGuitarRevisionAsync(int guitarId, int revisionId, int iteration, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(revisionId);
    if (revisionMetadata == null)
      return ServiceResult<GuitarDto>.Failure(400, $"Entity Revision ID '{revisionId}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<GuitarDto>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    if (revisionMetadata.Iteration != iteration)
      return ServiceResult<GuitarDto>.Failure(409, $"Cannot approve Entity Revision ID '{revisionId}' because it has changed since the approved review was loaded.");

    var oldParentRevisionId = revisionMetadata.ParentRevisionId;
    if (oldParentRevisionId == null)
      return ServiceResult<GuitarDto>.Failure(400, $"Entity Revision must have a Parent Revision ID when updating an existing Guitar.");

    var guitarRevisionEntity = await unitOfWork.GuitarRepository.GetGuitarRevisionEntityAsync(revisionId);
    if (guitarRevisionEntity == null)
      return ServiceResult<GuitarDto>.Failure(400, $"Guitar Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (guitarRevisionEntity.GuitarId != guitarId)
      return ServiceResult<GuitarDto>.Failure(400, $"Guitar ID {(guitarRevisionEntity.GuitarId == null ? "null" : guitarRevisionEntity.GuitarId)} on Guitar Revision and provided Guitar ID {guitarId} are different.");

    var currentGuitarRevisionVersioning = await unitOfWork.GuitarRepository.GetLatestGuitarRevisionVersionAsync(guitarId);
    if (currentGuitarRevisionVersioning == null)
      return ServiceResult<GuitarDto>.Failure(400, $"Revision version for Guitar ID '{guitarId}' does not exist");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to DB

    var guitar = await unitOfWork.GuitarRepository.UpdateGuitarAsync(guitarRevisionEntity);

    if (guitar == null)
      return ServiceResult<GuitarDto>.Failure(400, $"Guitar with ID {guitarId} does not exist");

    var approvalResult = await unitOfWork.RevisionMetadataRepository.ApprovePendingRevisionMetadataAsync(revisionId, userId, currentGuitarRevisionVersioning.Version);
    if (!approvalResult)
      return ServiceResult<GuitarDto>.Failure(400, $"Revision Metadata with ID {guitarRevisionEntity.Id} does not exist, or Version number {currentGuitarRevisionVersioning.Version} is not 1 or greater.");

    if (oldParentRevisionId != null)
    {
      await unitOfWork.RevisionMetadataRepository.AdoptPendingRevisionMetadatasAsync(oldParentRevisionId.Value, revisionId, userId);
    }

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<GuitarDto>.Failure(500, $"Could not update Guitar with ID {guitarId} for guitar revision with ID {guitarRevisionEntity.Id}");

    var guitarDto = new GuitarDto
    {
      Id = guitar.Id,
      Name = guitarRevisionEntity.Name,
      Description = guitarRevisionEntity.Description,
      Luthier = guitarRevisionEntity.Luthier == null ? null : new LuthierExerptDto
      {
        Id = guitarRevisionEntity.Luthier.Id,
        Name = guitarRevisionEntity.Luthier.Name,
      }
    };

    return ServiceResult<GuitarDto>.Success(200, guitarDto);
  }
}
