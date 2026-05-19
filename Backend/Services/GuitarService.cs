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

public class GuitarService : RevisableEntityService<
  int,
  Guitar,
  GuitarDto,
  GuitarSearchParams,
  GuitarExistsParams,
  GuitarRevision,
  GuitarRevisionSnapshotDto,
  GuitarCreateRevisionDto
>, IGuitarService
{
  public GuitarService(IUnitOfWork unitOfWork) : base(unitOfWork, "Guitar") { }

  protected override IRevisableEntityRepository<
    int,
    Guitar,
    GuitarDto,
    GuitarSearchParams,
    GuitarExistsParams,
    GuitarRevision,
    GuitarRevisionSnapshotDto,
    GuitarCreateRevisionDto
  >
    GetRepository() => UnitOfWork.GuitarRepository;

  protected override int? GetEntityId(GuitarRevision revisionEntity) => revisionEntity.GuitarId;

  protected override string GetEntityName(GuitarDto entity) => entity.Name;

  public override async Task<ServiceResult<GuitarRevisionSnapshotDto>> CreateInitialRevisionAsync(GuitarCreateRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims)
  {
    if (string.IsNullOrWhiteSpace(createGuitarRevisionDto.Name))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, "Name must be provided");

    var guitarExistsParams = new GuitarExistsParams
    {
      Name = createGuitarRevisionDto.Name
    };
    if (await UnitOfWork.GuitarRepository.ExistsAsync(guitarExistsParams))
    {
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar '{createGuitarRevisionDto.Name}' already exists");
    }

    if (createGuitarRevisionDto.LuthierId != null && !await UnitOfWork.LuthierRepository.ExistsByIdAsync(createGuitarRevisionDto.LuthierId.Value))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Luthier ID {createGuitarRevisionDto.LuthierId} does not exist.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = UnitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createGuitarRevisionDto.Comment, userId, null);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(500, "Unable to create Entity Revision Metadata for initial guitar.");

    var guitarRevision = UnitOfWork.GuitarRepository.CreateInitialRevision(revisionMetadata, createGuitarRevisionDto);

    if (!await UnitOfWork.SaveAllAsync())
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

  public override async Task<ServiceResult<GuitarRevisionSnapshotDto>> CreateRevisionAsync(int id, GuitarCreateRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims)
  {
    var currentGuitar = await UnitOfWork.GuitarRepository.GetByIdAsync(id);

    if (currentGuitar == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar ID '{id}' does not exist");

    var guitarExistsParams = new GuitarExistsParams
    {
      Name = createGuitarRevisionDto.Name,
      ExcludeId = id
    };
    if (!string.IsNullOrWhiteSpace(createGuitarRevisionDto.Name) && await UnitOfWork.GuitarRepository.ExistsAsync(guitarExistsParams))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar '{createGuitarRevisionDto.Name}' already exists.");

    if (createGuitarRevisionDto.LuthierId != null && !await UnitOfWork.LuthierRepository.ExistsByIdAsync(createGuitarRevisionDto.LuthierId.Value))
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

    var currentGuitarRevisionVersioning = await UnitOfWork.GuitarRepository.GetLatestRevisionVersionAsync(currentGuitar.Id);

    if (currentGuitarRevisionVersioning == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Revision version for Guitar ID '{id}' does not exist");

    var currentGuitarId = currentGuitarRevisionVersioning.Id; // this will be parentRevisionId for the new revision

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = UnitOfWork.RevisionMetadataRepository.CreateRevisionMetadataAsync(createGuitarRevisionDto.Comment, userId, currentGuitarId);

    if (!await UnitOfWork.SaveAllAsync())
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(500, $"Unable to create Entity Revision Metadata for Guitar ID '{id}'.");

    var guitarRevision = UnitOfWork.GuitarRepository.CreateRevision(id, revisionMetadata, createGuitarRevisionDto);

    if (!await UnitOfWork.SaveAllAsync())
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

  public override async Task<ServiceResult<GuitarRevisionSnapshotDto>> UpdateRevisionAsync(int revisionId, GuitarCreateRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims)
  {
    var userId = UserClaimsUtils.GetUserId(userClaims);
    var revisionSnapshot = await UnitOfWork.GuitarRepository.GetRevisionSnapshotAsync(revisionId, false, userId);
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
      currentGuitar = await UnitOfWork.GuitarRepository.GetByIdAsync(guitarId.Value);
      if (currentGuitar == null)
        return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar ID '{guitarId}' does not exist");
    }

    if (!string.IsNullOrWhiteSpace(createGuitarRevisionDto.Name))
    {
      var guitarExistsParams = new GuitarExistsParams
      {
        Name = createGuitarRevisionDto.Name,
        ExcludeId = guitarId
      };
      if (await UnitOfWork.GuitarRepository.ExistsAsync(guitarExistsParams))
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

    var guitarRevision = await UnitOfWork.GuitarRepository.UpdateRevisionAsync(revisionId, createGuitarRevisionDto);
    if (guitarRevision == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Guitar Revision ID '{revisionId}' does not exist.");

    var revisionMetadata = await UnitOfWork.RevisionMetadataRepository.UpdateRevisionMetadataAsync(revisionId, createGuitarRevisionDto.Comment, userId);
    if (revisionMetadata == null)
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Revision Metadata ID '{revisionId}' does not exist.");

    if (guitarRevision.LuthierId != null && !await UnitOfWork.LuthierRepository.ExistsByIdAsync(guitarRevision.LuthierId.Value))
      return ServiceResult<GuitarRevisionSnapshotDto>.Failure(400, $"Luthier ID {guitarRevision.LuthierId} does not exist.");

    if (!await UnitOfWork.SaveAllAsync())
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

  public override async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, int guitarId, HttpResponse response, ClaimsPrincipal user)
  {
    var guitar = await UnitOfWork.GuitarRepository.GetByIdAsync(guitarId);

    if (guitar == null)
      return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Failure(400, $"Guitar ID '{guitarId}' does not exist");

    var userIsModerator = user?.IsInRole("Moderator") == true;
    var guitarRevisions = await UnitOfWork.GuitarRepository.GetRevisionExcerptsAsync(revisionExcerptParams, guitarId, userIsModerator);
    response.AddPaginationHeader(guitarRevisions);
    response.Headers.Append("Guitar-Name", guitar.Name);
    response.Headers.AccessControlExposeHeaders = "Pagination, Guitar-Name";

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, guitarRevisions);
  }

  protected override RevisionDiffDto CalculateDiff(GuitarRevisionSnapshotDto oldRevision, GuitarRevisionSnapshotDto newRevision)
  {
    // if the guitar is initial version, then it wont have an ID.
    // but since guitar is initial version, there wont be two revisions of it to compare to.
    // we should never be able to get to this point if theres only an initial guitar revision,
    // so I'm suppressing the nullable warning 
    var diff = InitializeRevisionDiff(oldRevision.GuitarId!.Value, oldRevision, newRevision);

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

    return diff;
  }
}