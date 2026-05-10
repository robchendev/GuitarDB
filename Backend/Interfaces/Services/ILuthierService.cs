using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface ILuthierService
{
  /// <summary>
  /// Gets all luthiers.
  /// </summary>
  /// <param name="luthierParams">Pagination settings from user.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of luthier information.</returns>
  Task<PagedList<LuthierDto>> GetLuthiersAsync(LuthierParams luthierParams, HttpResponse response);

  /// <summary>
  /// Gets a luthier by their ID.
  /// </summary>
  /// <param name="id">The ID of the luthier.</param>
  /// <returns>The luthier information.</returns>
  Task<ServiceResult<LuthierDto>> GetLuthierAsync(int id);

  /// <summary>
  /// Creates a luthier revision for a luthier that doesn't exist yet.
  /// </summary>
  /// <param name="createLuthierRevisionDto">The details of the luthier.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created luthier revision snapshot.</returns>
  Task<ServiceResult<LuthierRevisionSnapshotDto>> CreateInitialLuthierRevisionAsync(CreateLuthierRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Creates a luthier revision for a luthier that already exists.
  /// </summary>
  /// <param name="id">The ID of the luthier to update.</param>
  /// <param name="createLuthierRevisionDto">The updated details of the luthier.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created luthier revision snapshot.</returns>
  Task<ServiceResult<LuthierRevisionSnapshotDto>> CreateLuthierRevisionAsync(int id, CreateLuthierRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Creates a luthier revision for a luthier that already exists.
  /// </summary>
  /// <param name="revisionId">The ID of the revision to update.</param>
  /// <param name="createLuthierRevisionDto">The updated details of the luthier.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created luthier revision snapshot.</returns>
  Task<ServiceResult<LuthierRevisionSnapshotDto>> UpdateLuthierRevisionAsync(int revisionId, CreateLuthierRevisionDto createLuthierRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Deletes a luthier from the database.
  /// </summary>
  /// <param name="id">The ID of the luthier to delete.</param>
  /// <returns>No content result.</returns>
  Task<ServiceResult<object>> DeleteLuthierAsync(int id);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for a Luthier. If the user is a moderator, will instead return excerpts regardless of status.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="luthierId">The ID of the Luthier to find revision metadata for.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetLuthierRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int luthierId, HttpResponse response, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for new Luthiers.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewLuthierRevisionExcerptsAsync(RevisionParams revisionExcerptParams, HttpResponse response);

  /// <summary>
  /// Retrieves the Snapshot for the Luthier Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Luthier Revision to retrieve the snapshot for.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="LuthierRevisionSnapshotDto"/></returns>
  Task<ServiceResult<LuthierRevisionSnapshotDto>> GetLuthierRevisionSnapshotAsync(int revisionId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a diff between two Luthier Revisions. The revisionId1 and revisionId2 order does not matter, the changes will always be compared from oldest to newest.
  /// </summary>
  /// <param name="revisionId1">The ID of a Luthier Revision to compare.</param>
  /// <param name="revisionId2">The ID of a Luthier Revision to compare, different from revisionId2.</param>
  /// <param name="luthierId">The ID of the Luthier both Revisions belong to.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="RevisionDiffDto"/></returns>
  Task<ServiceResult<RevisionDiffDto>> GetLuthierRevisionDiffAsync(int revisionId1, int revisionId2, int luthierId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves a Luthier Revision that doesn't isn't tied to a Luthier ID, creating a new Luthier.
  /// </summary>
  /// <param name="revisionId">The ID of the Luthier Revision to create a Luthier with.</param>
  /// <param name="iteration">The iteration number of the Luthier Revision to check for conflicting data.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="LuthierDto"/></returns>
  Task<ServiceResult<LuthierDto>> ApproveCreateLuthierRevisionAsync(int revisionId, int iteration, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves a Luthier Revision that is tied to a Luthier ID, updating the existing Luthier.
  /// </summary>
  /// <param name="luthierId">The ID of the Luthier to update.</param>
  /// <param name="revisionId">The ID of the Luthier Revision to update the Luthier with.</param>
  /// <param name="iteration">The iteration number of the Luthier Revision to check for conflicting data.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="LuthierDto"/></returns>
  Task<ServiceResult<LuthierDto>> ApproveUpdateLuthierRevisionAsync(int luthierId, int revisionId, int iteration, ClaimsPrincipal userClaims);
}