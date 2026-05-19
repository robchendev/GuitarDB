using System.Security.Claims;
using Backend.Common;
using Backend.Common.Interfaces;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.DTOs.Interfaces;
using Backend.Entities.Abstract;
using Backend.Extensions;
using Backend.Guards;
using Backend.Interfaces;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;

namespace Backend.Services.Abstract;

public abstract class RevisableEntityService<
  TID,
  TEntity,
  TDto,
  TSearchParams,
  TExistsParams,
  TRevisionEntity,
  TRevisionSnapshotDto,
  TCreateDto
>(
  IUnitOfWork unitOfWork,
  string entitySingular
// string entityPlural
) : IRevisableEntityService<
  TDto,
  TSearchParams,
  TRevisionSnapshotDto,
  TCreateDto
>
  where TRevisionSnapshotDto : IRevisionSnapshotDto
  where TSearchParams : PaginationParams
  where TExistsParams : EntityExistsParams
  where TRevisionEntity : IRevisionEntity
  where TEntity : IHasId
{
  protected IUnitOfWork UnitOfWork => unitOfWork;
  protected string EntitySingular => entitySingular;

  protected abstract IRevisableEntityRepository<
    TID,
    TEntity,
    TDto,
    TSearchParams,
    TExistsParams,
    TRevisionEntity,
    TRevisionSnapshotDto,
    TCreateDto
  > GetRepository();
  protected abstract RevisionDiffDto CalculateDiff(TRevisionSnapshotDto oldRevision, TRevisionSnapshotDto newRevision);
  protected abstract int? GetEntityId(TRevisionEntity revisionEntity);
  protected abstract string GetEntityName(TDto entity);

  // These MUST be implemented by every subclass or it won't compile
  public abstract Task<ServiceResult<TRevisionSnapshotDto>> CreateInitialRevisionAsync(TCreateDto createDto, ClaimsPrincipal userClaims);
  public abstract Task<ServiceResult<TRevisionSnapshotDto>> CreateRevisionAsync(int id, TCreateDto createDto, ClaimsPrincipal userClaims);
  public abstract Task<ServiceResult<TRevisionSnapshotDto>> UpdateRevisionAsync(int revisionId, TCreateDto createDto, ClaimsPrincipal userClaims);
  public abstract Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int entityId, HttpResponse response, ClaimsPrincipal userClaims);

  public async Task<PagedList<TDto>> GetAllAsync(TSearchParams searchParams, HttpResponse response)
  {
    var entities = await GetRepository().GetAllAsync(searchParams);
    response.AddPaginationHeader(entities);

    return entities;
  }

  public async Task<ServiceResult<TDto>> GetByIdAsync(int id)
  {
    var entity = await GetRepository().GetByIdAsync(id);

    if (entity == null)
      return ServiceResult<TDto>.Failure(404, $"{entitySingular} with ID {id} not found");

    return ServiceResult<TDto>.Success(200, entity);
  }

  public async Task<ServiceResult<object>> DeleteAsync(int id)
  {
    var exists = await GetRepository().DeleteAsync(id);
    if (!exists)
      return ServiceResult<object>.Failure(400, $"{entitySingular} ID '{id}' does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<object>.Failure(500, $"Could not delete {entitySingular} (other entities may reference it)");

    return ServiceResult<object>.Success(200, null);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, HttpResponse response)
  {
    var revisions = await GetRepository().GetNewRevisionExcerptsAsync(revisionExcerptParams);
    response.AddPaginationHeader(revisions);

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, revisions);
  }

  public async Task<ServiceResult<TRevisionSnapshotDto>> GetRevisionSnapshotAsync(int revisionId, ClaimsPrincipal user)
  {
    var userIsModerator = user.IsInRole("Moderator");
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var revisionSnapshot = await GetRepository().GetRevisionSnapshotAsync(revisionId, userIsModerator, userId);

    if (revisionSnapshot == null)
      return ServiceResult<TRevisionSnapshotDto>.Failure(404, $"Revision ID '{revisionId}' not found or user has no access");

    return ServiceResult<TRevisionSnapshotDto>.Success(200, revisionSnapshot);
  }

  public async Task<ServiceResult<RevisionDiffDto>> GetRevisionDiffAsync(int revisionId1, int revisionId2, int entityId, ClaimsPrincipal user)
  {
    if (revisionId1 == revisionId2)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Revision ID '{revisionId1}' cannot be compared with itself");

    var userIsModerator = user.IsInRole("Moderator");
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var revision1 = await GetRepository().GetRevisionSnapshotAsync(revisionId1, userIsModerator, userId);
    if (revision1 == null)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Revision ID '{revisionId1}' not found or user has no access.");

    var revision2 = await GetRepository().GetRevisionSnapshotAsync(revisionId2, userIsModerator, userId);
    if (revision2 == null)
      return ServiceResult<RevisionDiffDto>.Failure(400, $"Revision ID '{revisionId2}' not found or user has no access.");

    bool rev1Older = revision1.CreatedAt <= revision2.CreatedAt;
    var oldRevision = rev1Older ? revision1 : revision2;
    var newRevision = rev1Older ? revision2 : revision1;
    var diff = CalculateDiff(oldRevision, newRevision);

    return ServiceResult<RevisionDiffDto>.Success(200, diff);
  }

  public async Task<ServiceResult<TDto>> ApproveCreateRevisionAsync(int revisionId, uint rowVersion, ClaimsPrincipal userClaims)
  {
    var revisionGuard = await RevisionGuards.ApproveCreateRevisionGuardAsync<TDto>(unitOfWork, revisionId, rowVersion);
    if (revisionGuard.Failure != null)
      return revisionGuard.Failure;

    var revisionEntity = await GetRepository().GetRevisionEntityAsync(revisionId);
    if (revisionEntity == null)
      return ServiceResult<TDto>.Failure(400, $"Revision for {entitySingular} Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (GetEntityId(revisionEntity) != null)
      return ServiceResult<TDto>.Failure(400, $"Cannot create {entitySingular} because Revision with ID '{revisionId}' is tied to an existing {entitySingular} with ID '{GetEntityId(revisionEntity)}'.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var entity = GetRepository().Create(revisionEntity);

    var approvalResult = await unitOfWork.RevisionMetadataRepository.ApprovePendingRevisionMetadataAsync(revisionEntity.Id, userId, null);
    if (!approvalResult)
      return ServiceResult<TDto>.Failure(400, $"Revision Metadata with ID {revisionEntity.Id} does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<TDto>.Failure(500, $"Could not create {entitySingular} using Revision ID {revisionEntity.Id}.");

    var entityDto = await GetRepository().GetByIdAsync(entity.Id);

    return ServiceResult<TDto>.Success(200, entityDto);
  }

  public async Task<ServiceResult<TDto>> ApproveUpdateRevisionAsync(int entityId, int revisionId, uint rowVersion, ClaimsPrincipal userClaims)
  {
    var revisionGuard = await RevisionGuards.ApproveUpdateRevisionGuardAsync<TDto>(unitOfWork, revisionId, rowVersion);
    if (revisionGuard.Failure != null)
      return revisionGuard.Failure;

    var revisionEntity = await GetRepository().GetRevisionEntityAsync(revisionId);
    if (revisionEntity == null)
      return ServiceResult<TDto>.Failure(400, $"Revision for {entitySingular} Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (GetEntityId(revisionEntity) != entityId)
      return ServiceResult<TDto>.Failure(400, $"{entitySingular} ID '{GetEntityId(revisionEntity)}' on Revision and provided {entitySingular} ID '{entityId}' are different.");

    var currentRevisionVersioning = await GetRepository().GetLatestRevisionVersionAsync(entityId);
    if (currentRevisionVersioning == null)
      return ServiceResult<TDto>.Failure(400, $"Revision version for {entitySingular} ID '{entityId}' does not exist");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var entity = await GetRepository().UpdateAsync(revisionEntity);
    if (entity == null)
      return ServiceResult<TDto>.Failure(400, $"{entitySingular} with ID '{entityId}' does not exist");

    var approvalResult = await unitOfWork.RevisionMetadataRepository.ApprovePendingRevisionMetadataAsync(revisionId, userId, currentRevisionVersioning.Version);
    if (!approvalResult)
      return ServiceResult<TDto>.Failure(400, $"Revision Metadata with ID {revisionEntity.Id} does not exist, or Version number {currentRevisionVersioning.Version} is not 1 or greater.");

    if (revisionGuard.ParentRevisionId != null)
      await unitOfWork.RevisionMetadataRepository.AdoptPendingRevisionMetadatasAsync(revisionGuard.ParentRevisionId.Value, revisionId, userId);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<TDto>.Failure(500, $"Could not update {entitySingular} with ID '{entityId}' for revision with ID '{revisionEntity.Id}'");

    var entityDto = await GetRepository().GetByIdAsync(entity.Id);

    return ServiceResult<TDto>.Success(200, entityDto);
  }

  protected RevisionDiffDto InitializeRevisionDiff(
    int entityId,
    TRevisionSnapshotDto oldRevision,
    TRevisionSnapshotDto newRevision)
  {
    var diff = new RevisionDiffDto { EntityId = entityId };
    diff.Changes["id"] = new Change { Old = oldRevision.Id, New = newRevision.Id };
    diff.Changes["createdAt"] = new Change { Old = oldRevision.CreatedAt, New = newRevision.CreatedAt };
    diff.Changes["createdBy"] = new Change { Old = oldRevision.CreatedBy, New = newRevision.CreatedBy };
    diff.Changes["version"] = new Change { Old = oldRevision.Version, New = newRevision.Version };
    diff.Changes["comment"] = new Change { Old = oldRevision.Comment, New = newRevision.Comment };
    diff.RowVersion = newRevision.RowVersion;

    return diff;
  }
}