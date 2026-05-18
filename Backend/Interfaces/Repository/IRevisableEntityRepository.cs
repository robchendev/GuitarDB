using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities.Revision;

namespace Backend.Interfaces.Repository;

public interface IRevisableEntityRepository<
  TEntity,
  TDto,
  TSearchParams,
  TExistsParams,
  TRevisionEntity,
  TRevisionSnapshotDto,
  TCreateDto
>
  where TSearchParams : PaginationParams
{
  /// <summary>
  /// Retrieves a entity by its unique ID.
  /// </summary>
  /// <param name="id">The ID of the entity to retrieve.</param>
  /// <returns><typeparamref name="TDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<TDto?> GetByIdAsync(int id);

  /// <summary>
  /// Retrieves a list of entities.
  /// </summary>
  /// <param name="params">Query params for the entity.</param>
  /// <returns>A paginated list of <typeparamref name="TDto"/> objects.</returns>
  Task<PagedList<TDto>> GetAllAsync(TSearchParams @params);

  /// <summary>
  /// Checks if an entity with the matching params exists.
  /// </summary>
  /// <param name="params">Query params for the entity.</param>
  /// <returns><c>true</c> if a matching entity is found; otherwise, <c>false</c>.</returns>
  Task<bool> ExistsAsync(TExistsParams @params);

  /// <summary>
  /// Checks if an entity with the matching ID exists.
  /// </summary>
  /// <param name="id">The ID of the entity.</param>
  /// <returns><c>true</c> if an entity with the ID exists; otherwise, <c>false</c>.</returns>
  Task<bool> ExistsByIdAsync(int id);

  /// <summary>
  /// Retrieves the Snapshot for the entity Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the entity Revision.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the entity regardless of Status if true.</param>
  /// <param name="userId">A possibly null ID of the user requesting the Snapshot.</param>
  /// <returns><typeparamref name="TRevisionSnapshotDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<TRevisionSnapshotDto?> GetRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null);

  /// <summary>
  /// Retrieves the database entity of the entity Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the entity Revision.</param>
  /// <returns><typeparamref name="TRevisionEntity"/> if found; otherwise, <c>null</c>.</returns>
  Task<TRevisionEntity?> GetRevisionEntityAsync(int revisionId);

  /// <summary>
  /// Retrieves the current (AKA latest version) committed entity Revision.
  /// </summary>
  /// <param name="entityId">The ID of the entity.</param>
  /// <returns><see cref="RevisionMetadataVersioningDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<RevisionMetadataVersioningDto?> GetLatestRevisionVersionAsync(int entityId);

  /// <summary>
  /// Deletes an entity from the database.
  /// </summary>
  /// <param name="id">The ID of the entity to delete.</param>
  /// <returns><c>true</c> if entity found; otherwise, <c>false</c>.</returns>
  Task<bool> DeleteAsync(int id);

  /// <summary>
  /// Retrieves a list of RevisionMetadata excerpts for an entity.
  /// </summary>
  /// <param name="revisionParams">Query params for revision excerpts.</param>
  /// <param name="entityId">The ID of the entity.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the entity regardless of Status if true.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/>.</returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetRevisionExcerptsAsync(RevisionParams revisionParams, int entityId, bool userIsModerator);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for all new entities.
  /// </summary>
  /// <param name="revisionParams">Query params for revision excerpts.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/>.</returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetNewRevisionExcerptsAsync(RevisionParams revisionParams);

  /// <summary>
  /// Creates a new entity Revision, that when approved, will create a new entity.
  /// </summary>
  /// <param name="metadata">The RevisionMetadata to tie this entity Revision to.</param>
  /// <param name="dto">The details of the new entity.</param>
  /// <returns><typeparamref name="TRevisionEntity"/></returns>
  TRevisionEntity CreateInitialRevision(RevisionMetadata metadata, TCreateDto dto);

  /// <summary>
  /// Creates a new entity Revision for an existing entity, that when approved, will update the existing entity.
  /// </summary>
  /// <param name="entityId">The ID of the entity.</param>
  /// <param name="metadata">The RevisionMetadata to tie this entity Revision to.</param>
  /// <param name="dto">The details of the updated entity.</param>
  /// <returns><typeparamref name="TRevisionEntity"/></returns>
  TRevisionEntity CreateRevision(int entityId, RevisionMetadata metadata, TCreateDto dto);

  /// <summary>
  /// Updates an entity Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the entity Revision.</param>
  /// <param name="dto">The details of the updated entity.</param>
  /// <returns><typeparamref name="TRevisionEntity"/> if found; otherwise, <c>null</c>.</returns>
  Task<TRevisionEntity?> UpdateRevisionAsync(int revisionId, TCreateDto dto);

  /// <summary>
  /// Creates an entity from a Revision.
  /// </summary>
  /// <param name="revision">The revision snapshot of the entity.</param>
  /// <returns><typeparamref name="TEntity"/></returns>
  TEntity Create(TRevisionEntity revision);

  /// <summary>
  /// Updates an entity's details from a Revision.
  /// </summary>
  /// <param name="revision">The revision snapshot of the entity.</param>
  /// <returns><typeparamref name="TEntity"/> if found; otherwise, <c>null</c>.</returns>
  Task<TEntity?> UpdateAsync(TRevisionEntity revision);
}