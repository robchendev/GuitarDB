using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IGuitarService
{
  /// <summary>
  /// Gets all guitars.
  /// </summary>
  /// <param name="guitarParams">Pagination settings from user.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of guitar information.</returns>
  Task<PagedList<GuitarDto>> GetGuitarsAsync(GuitarParams guitarParams, HttpResponse response);

  /// <summary>
  /// Gets a guitar by their ID.
  /// </summary>
  /// <param name="id">The ID of the guitar.</param>
  /// <returns>The guitar information.</returns>
  Task<ServiceResult<GuitarDto>> GetGuitarAsync(int id);

  /// <summary>
  /// Creates a guitar revision for a guitar that doesn't exist yet.
  /// </summary>
  /// <param name="createGuitarRevisionDto">The details of the guitar.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created guitar revision snapshot.</returns>
  Task<ServiceResult<GuitarRevisionSnapshotDto>> CreateInitialGuitarRevisionAsync(CreateGuitarRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Creates a guitar revision for a guitar that already exists.
  /// </summary>
  /// <param name="id">The ID of the guitar to update.</param>
  /// <param name="createGuitarRevisionDto">The updated details of the guitar.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created guitar revision snapshot.</returns>
  Task<ServiceResult<GuitarRevisionSnapshotDto>> CreateGuitarRevisionAsync(int id, CreateGuitarRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Creates a guitar revision for a guitar that already exists.
  /// </summary>
  /// <param name="revisionId">The ID of the revision to update.</param>
  /// <param name="createGuitarRevisionDto">The updated details of the guitar.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created guitar revision snapshot.</returns>
  Task<ServiceResult<GuitarRevisionSnapshotDto>> UpdateGuitarRevisionAsync(int revisionId, CreateGuitarRevisionDto createGuitarRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Deletes a guitar from the database.
  /// </summary>
  /// <param name="id">The ID of the guitar to delete.</param>
  /// <returns>No content result.</returns>
  Task<ServiceResult<object>> DeleteGuitarAsync(int id);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for a Guitar. If the user is a moderator, will instead return excerpts regardless of status.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="guitarId">The ID of the Guitar to find revision metadata for.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetGuitarRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int guitarId, HttpResponse response, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for new Guitars.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewGuitarRevisionExcerptsAsync(RevisionParams revisionExcerptParams, HttpResponse response);

  /// <summary>
  /// Retrieves the Snapshot for the Guitar Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Guitar Revision to retrieve the snapshot for.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="GuitarRevisionSnapshotDto"/></returns>
  Task<ServiceResult<GuitarRevisionSnapshotDto>> GetGuitarRevisionSnapshotAsync(int revisionId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a diff between two Guitar Revisions. The revisionId1 and revisionId2 order does not matter, the changes will always be compared from oldest to newest.
  /// </summary>
  /// <param name="revisionId1">The ID of a Guitar Revision to compare.</param>
  /// <param name="revisionId2">The ID of a Guitar Revision to compare, different from revisionId2.</param>
  /// <param name="guitarId">The ID of the Guitar both Revisions belong to.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="GuitarRevisionDiffDto"/></returns>
  Task<ServiceResult<GuitarRevisionDiffDto>> GetGuitarRevisionDiffAsync(int revisionId1, int revisionId2, int guitarId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves a Guitar Revision that doesn't isn't tied to a Guitar ID, creating a new Guitar.
  /// </summary>
  /// <param name="revisionId">The ID of the Guitar Revision to create a Guitar with.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="GuitarDto"/></returns>
  Task<ServiceResult<GuitarDto>> ApproveCreateGuitarRevisionAsync(int revisionId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves a Guitar Revision that is tied to a Guitar ID, updating the existing Guitar.
  /// </summary>
  /// <param name="guitarId">The ID of the Guitar to update.</param>
  /// <param name="revisionId">The ID of the Guitar Revision to update the Guitar with.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="GuitarDto"/></returns>
  Task<ServiceResult<GuitarDto>> ApproveUpdateGuitarRevisionAsync(int guitarId, int revisionId, ClaimsPrincipal userClaims);
}