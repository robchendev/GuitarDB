using System.Diagnostics;
using Backend.Common;
using Backend.Enums;
using Backend.Interfaces;

namespace Backend.Guards;

public sealed record UpdateRevisionGuardResult<T>(ServiceResult<T>? Failure, int? ParentRevisionId);
public sealed record CreateRevisionGuardResult<T>(ServiceResult<T>? Failure);

public static class RevisionGuards
{
  [DebuggerHidden]
  public static async Task<UpdateRevisionGuardResult<T>> ApproveUpdateRevisionGuardAsync<T>(IUnitOfWork unitOfWork, int revisionId, uint rowVersion)
  {
    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(revisionId);

    if (revisionMetadata == null)
      return new UpdateRevisionGuardResult<T>(ServiceResult<T>.Failure(400, $"Entity Revision ID '{revisionId}' not found"), null);

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return new UpdateRevisionGuardResult<T>(ServiceResult<T>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'."), null);

    if (revisionMetadata.RowVersion != rowVersion)
      return new UpdateRevisionGuardResult<T>(ServiceResult<T>.Failure(409, $"Cannot approve Entity Revision ID '{revisionId}' because it has changed since the approved review was loaded."), null);

    var oldParentRevisionId = revisionMetadata.ParentRevisionId;

    if (oldParentRevisionId == null)
      return new UpdateRevisionGuardResult<T>(ServiceResult<T>.Failure(400, $"Entity Revision must have a Parent Revision ID when updating an existing entity."), null);

    return new UpdateRevisionGuardResult<T>(null, oldParentRevisionId.Value);
  }

  [DebuggerHidden]
  public static async Task<CreateRevisionGuardResult<T>> ApproveCreateRevisionGuardAsync<T>(IUnitOfWork unitOfWork, int revisionId, uint rowVersion)
  {
    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(revisionId);

    if (revisionMetadata == null)
      return new CreateRevisionGuardResult<T>(ServiceResult<T>.Failure(400, $"Entity Revision ID '{revisionId}' not found"));

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return new CreateRevisionGuardResult<T>(ServiceResult<T>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'."));

    if (revisionMetadata.RowVersion != rowVersion)
      return new CreateRevisionGuardResult<T>(ServiceResult<T>.Failure(409, $"Cannot approve Entity Revision ID '{revisionId}' because it has changed since the approved review was loaded."));

    var parentRevisionId = revisionMetadata.ParentRevisionId;
    if (parentRevisionId != null)
      return new CreateRevisionGuardResult<T>(ServiceResult<T>.Failure(400, $"Entity Revision cannot have a Parent Revision ID when creating a new Luthier. Current Parent Revision ID: {parentRevisionId}"));

    return new CreateRevisionGuardResult<T>(null);
  }
}