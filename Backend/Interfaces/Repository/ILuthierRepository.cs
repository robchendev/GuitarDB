using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;

namespace Backend.Interfaces.Repository;

public interface ILuthierRepository
{
  /// <summary>
  /// Retrieves a list of luthiers.
  /// </summary>
  /// <param name="luthierParams">Query params for luthiers.</param>
  /// <returns>A paginated list of <see cref="LuthierDto"/> objects.</returns>
  Task<PagedList<LuthierDto>> GetLuthiersAsync(LuthierParams luthierParams);

  /// <summary>
  /// Retrieves a luthier by its unique ID.
  /// </summary>
  /// <param name="id">The ID of the luthier to retrieve.</param>
  /// <returns><see cref="LuthierDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<LuthierDto?> GetLuthierByIdAsync(int id);

  /// <summary>
  /// Creates a luthier.
  /// </summary>
  /// <param name="luthierRevision">The revision snapshot of the luthier.</param>
  /// <returns><see cref="Luthier"/></returns>
  Luthier CreateLuthier(LuthierRevision luthierRevision);

  /// <summary>
  /// Updates a luthier's details.
  /// </summary>
  /// <param name="luthierRevision">The revision snapshot of the luthier.</param>
  /// <returns><see cref="Luthier"/> if found; otherwise, <c>null</c>.</returns>
  Task<Luthier?> UpdateLuthierAsync(LuthierRevision luthierRevision);

  /// <summary>
  /// Deletes a luthier from the database.
  /// </summary>
  /// <param name="id">The ID of the luthier to delete.</param>
  /// <returns><c>true</c> if Luthier found; otherwise, <c>false</c>.</returns>
  Task<bool> DeleteLuthierAsync(int id);

  /// <summary>
  /// Checks if a luthier exists
  /// </summary>
  /// <param name="name">The name of the luthier.</param>
  /// <param name="brandId">The brand ID of the luthier.</param>
  /// <param name="luthierId">The luthier ID the luthier.</param>
  /// <param name="excludeId">The ID of the luthier to exclude from the check.</param>
  /// <returns><c>true</c> if luthier of matching name+brand or name+luthier found; otherwise, <c>false</c>.</returns>
  Task<bool> LuthierExistsAsync(string name, int? brandId = null, int? luthierId = null, int? excludeId = null);

  /// <summary>
  /// Checks if a luthier with the ID exists.
  /// </summary>
  /// <param name="id">The ID of the Luthier.</param>
  /// <returns><c>true</c> if a luthier with the ID exists; otherwise, <c>false</c>.</returns>
  Task<bool> LuthierExistsByIdAsync(int id);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for a Luthier.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="luthierId">The ID of the Luthier.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the Luthier regardless of Status if true.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetLuthierRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int luthierId, bool userIsModerator);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for all new Luthiers.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetNewLuthierRevisionExcerptsAsync(RevisionParams revisionExcerptParams);

  /// <summary>
  /// Retrieves the Snapshot for the Luthier Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Luthier Revision.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the Luthier regardless of Status if true.</param>
  /// <param name="userId">A possibly null ID of the user requesting the Snapshot.</param>
  /// <returns><see cref="LuthierRevisionSnapshotDto"/></returns>
  Task<LuthierRevisionSnapshotDto?> GetLuthierRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null);

  /// <summary>
  /// Retrieves the database entity of the Luthier Revision. 
  /// </summary>
  /// <param name="revisionId">The ID of the Luthier Revision.</param>
  /// <returns><see cref="LuthierRevision"/></returns>
  Task<LuthierRevision?> GetLuthierRevisionEntityAsync(int revisionId);

  /// <summary>
  /// Retrieves the current (AKA latest version) committed Luthier Revision.
  /// </summary>
  /// <param name="luthierId">The ID of the Luthier.</param>
  /// <returns><see cref="RevisionMetadataVersioningDto"/></returns>
  Task<RevisionMetadataVersioningDto?> GetLatestLuthierRevisionVersionAsync(int luthierId);

  /// <summary>
  /// Creates a new Luthier Revision, that when approved, will create a new Luthier.
  /// </summary>
  /// <param name="revisionMetadata">The RevisionMetadata to tie this Luthier Revision to.</param>
  /// <param name="createLuthierRevisionDto">The details of the new Luthier.</param>
  /// <returns><see cref="LuthierRevision"/></returns>
  LuthierRevision CreateInitialLuthierRevisionAsync(RevisionMetadata revisionMetadata, CreateLuthierRevisionDto createLuthierRevisionDto);

  /// <summary>
  /// Creates a new Luthier Revision for an existing Luthier, that when approved, will update the existing Luthier.
  /// </summary>
  /// <param name="luthierId">The ID of the Luthier.</param>
  /// <param name="revisionMetadata">The RevisionMetadata to tie this Luthier Revision to.</param>
  /// <param name="createLuthierRevisionDto">The details of the updated Luthier.</param>
  /// <returns><see cref="LuthierRevision"/></returns>
  LuthierRevision CreateLuthierRevisionAsync(int luthierId, RevisionMetadata revisionMetadata, CreateLuthierRevisionDto createLuthierRevisionDto);

  /// <summary>
  /// Updates a Luthier Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Luthier Revision.</param>
  /// <param name="createLuthierRevisionDto">The details of the updated Luthier.</param>
  /// <returns><see cref="LuthierRevision"/> if found; otherwise, <c>null</c></returns>
  Task<LuthierRevision?> UpdateLuthierRevisionAsync(int revisionId, CreateLuthierRevisionDto createLuthierRevisionDto);
}
