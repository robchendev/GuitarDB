using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IRevisableEntityService<
  TDto,
  TSearchParams,
  TRevisionSnapshotDto,
  TCreateDto
>
  where TSearchParams : PaginationParams
{
  /// <summary>
  /// Gets all entities.
  /// </summary>
  /// <param name="searchParams">Pagination and search settings from user.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of entity information.</returns>
  Task<PagedList<TDto>> GetAllAsync(TSearchParams searchParams, HttpResponse response);

  /// <summary>
  /// Gets an entity by its ID.
  /// </summary>
  /// <param name="id">The ID of the entity.</param>
  /// <returns>The entity information.</returns>
  Task<ServiceResult<TDto>> GetByIdAsync(int id);

  /// <summary>
  /// Creates an entity revision for an entity that doesn't exist yet.
  /// </summary>
  /// <param name="dto">The details of the entity.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created entity revision snapshot.</returns>
  Task<ServiceResult<TRevisionSnapshotDto>> CreateInitialRevisionAsync(TCreateDto dto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Creates an entity revision for an entity that already exists.
  /// </summary>
  /// <param name="id">The ID of the entity to update.</param>
  /// <param name="dto">The updated details of the entity.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created entity revision snapshot.</returns>
  Task<ServiceResult<TRevisionSnapshotDto>> CreateRevisionAsync(int id, TCreateDto dto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Updates an existing entity revision.
  /// </summary>
  /// <param name="revisionId">The ID of the revision to update.</param>
  /// <param name="dto">The updated details of the entity.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The updated entity revision snapshot.</returns>
  Task<ServiceResult<TRevisionSnapshotDto>> UpdateRevisionAsync(int revisionId, TCreateDto dto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Deletes an entity from the database.
  /// </summary>
  /// <param name="id">The ID of the entity to delete.</param>
  /// <returns>No content result.</returns>
  Task<ServiceResult<object>> DeleteAsync(int id);

  /// <summary>
  /// Retrieves a paginated list of RevisionMetadata excerpts for an entity. If the user is a moderator, will instead return excerpts regardless of status.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revision excerpts.</param>
  /// <param name="entityId">The ID of the entity to find revision metadata for.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/>.</returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int entityId, HttpResponse response, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a paginated list of pending RevisionMetadata excerpts for all new entities.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revision excerpts.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/>.</returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewRevisionExcerptsAsync(RevisionParams revisionExcerptParams, HttpResponse response);

  /// <summary>
  /// Retrieves the snapshot for an entity Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the entity Revision to retrieve the snapshot for.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><typeparamref name="TRevisionSnapshotDto"/></returns>
  Task<ServiceResult<TRevisionSnapshotDto>> GetRevisionSnapshotAsync(int revisionId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a diff between two entity Revisions. The revisionId1 and revisionId2 order does not matter, the changes will always be compared from oldest to newest.
  /// </summary>
  /// <param name="revisionId1">The ID of an entity Revision to compare.</param>
  /// <param name="revisionId2">The ID of an entity Revision to compare, different from revisionId1.</param>
  /// <param name="entityId">The ID of the entity both Revisions belong to.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="RevisionDiffDto"/></returns>
  Task<ServiceResult<RevisionDiffDto>> GetRevisionDiffAsync(int revisionId1, int revisionId2, int entityId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves an entity Revision that is not tied to an entity ID, creating a new entity.
  /// </summary>
  /// <param name="revisionId">The ID of the entity Revision to create an entity with.</param>
  /// <param name="rowVersion">The row version of the entity Revision to check for conflicting data.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><typeparamref name="TDto"/></returns>
  Task<ServiceResult<TDto>> ApproveCreateRevisionAsync(int revisionId, uint rowVersion, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves an entity Revision that is tied to an entity ID, updating the existing entity.
  /// </summary>
  /// <param name="entityId">The ID of the entity to update.</param>
  /// <param name="revisionId">The ID of the entity Revision to update the entity with.</param>
  /// <param name="rowVersion">The row version of the entity Revision to check for conflicting data.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><typeparamref name="TDto"/></returns>
  Task<ServiceResult<TDto>> ApproveUpdateRevisionAsync(int entityId, int revisionId, uint rowVersion, ClaimsPrincipal userClaims);
}