using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;

namespace Backend.Interfaces.Repository;

public interface IGuitarRepository
{
  /// <summary>
  /// Retrieves a list of guitars.
  /// </summary>
  /// <param name="guitarParams">Query params for guitars.</param>
  /// <returns>A paginated list of <see cref="GuitarDto"/> objects.</returns>
  Task<PagedList<GuitarDto>> GetGuitarsAsync(GuitarParams guitarParams);

  /// <summary>
  /// Retrieves a guitar by its unique ID.
  /// </summary>
  /// <param name="id">The ID of the guitar to retrieve.</param>
  /// <returns><see cref="GuitarDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<GuitarDto?> GetGuitarByIdAsync(int id);

  /// <summary>
  /// Creates a guitar.
  /// </summary>
  /// <param name="guitarRevision">The revision snapshot of the guitar.</param>
  /// <returns><see cref="Guitar"/></returns>
  Guitar CreateGuitar(GuitarRevision guitarRevision);

  /// <summary>
  /// Updates a guitar's details.
  /// </summary>
  /// <param name="guitarRevision">The revision snapshot of the guitar.</param>
  /// <returns><see cref="Guitar"/> if found; otherwise, <c>null</c>.</returns>
  Task<Guitar?> UpdateGuitarAsync(GuitarRevision guitarRevision);

  /// <summary>
  /// Deletes a guitar from the database.
  /// </summary>
  /// <param name="id">The ID of the guitar to delete.</param>
  /// <returns><c>true</c> if Guitar found; otherwise, <c>false</c>.</returns>
  Task<bool> DeleteGuitarAsync(int id);

  /// <summary>
  /// Checks if a guitar exists
  /// </summary>
  /// <param name="name">The name of the guitar.</param>
  /// <param name="excludeId">The ID of the guitar to exclude from the check.</param>
  /// <returns><c>true</c> if guitar of matching name and location address found; otherwise, <c>false</c>.</returns>
  Task<bool> GuitarExistsAsync(string name, int? excludeId = null);

  /// <summary>
  /// Checks if a guitar with the ID exists.
  /// </summary>
  /// <param name="id">The ID of the Guitar.</param>
  /// <returns><c>true</c> if a guitar with the ID exists; otherwise, <c>false</c>.</returns>
  Task<bool> GuitarExistsByIdAsync(int id);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for a Guitar.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="guitarId">The ID of the Guitar.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the Guitar regardless of Status if true.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetGuitarRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int guitarId, bool userIsModerator);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for all new Guitars.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetNewGuitarRevisionExcerptsAsync(RevisionParams revisionExcerptParams);

  /// <summary>
  /// Retrieves the Snapshot for the Guitar Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Guitar Revision.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the Guitar regardless of Status if true.</param>
  /// <param name="userId">A possibly null ID of the user requesting the Snapshot.</param>
  /// <returns><see cref="GuitarRevisionSnapshotDto"/></returns>
  Task<GuitarRevisionSnapshotDto?> GetGuitarRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null);

  /// <summary>
  /// Retrieves the database entity of the Guitar Revision. 
  /// </summary>
  /// <param name="revisionId">The ID of the Guitar Revision.</param>
  /// <returns><see cref="GuitarRevision"/></returns>
  Task<GuitarRevision?> GetGuitarRevisionEntityAsync(int revisionId);

  /// <summary>
  /// Retrieves the current (AKA latest version) committed Guitar Revision.
  /// </summary>
  /// <param name="guitarId">The ID of the Guitar.</param>
  /// <returns><see cref="RevisionMetadataVersioningDto"/></returns>
  Task<RevisionMetadataVersioningDto?> GetLatestGuitarRevisionVersionAsync(int guitarId);

  /// <summary>
  /// Creates a new Guitar Revision, that when approved, will create a new Guitar.
  /// </summary>
  /// <param name="revisionMetadata">The RevisionMetadata to tie this Guitar Revision to.</param>
  /// <param name="createGuitarRevisionDto">The details of the new Guitar.</param>
  /// <returns><see cref="GuitarRevision"/></returns>
  GuitarRevision CreateInitialGuitarRevisionAsync(RevisionMetadata revisionMetadata, CreateGuitarRevisionDto createGuitarRevisionDto);

  /// <summary>
  /// Creates a new Guitar Revision for an existing Guitar, that when approved, will update the existing Guitar.
  /// </summary>
  /// <param name="guitarId">The ID of the Guitar.</param>
  /// <param name="revisionMetadata">The RevisionMetadata to tie this Guitar Revision to.</param>
  /// <param name="createGuitarRevisionDto">The details of the updated Guitar.</param>
  /// <returns><see cref="GuitarRevision"/></returns>
  GuitarRevision CreateGuitarRevisionAsync(int guitarId, RevisionMetadata revisionMetadata, CreateGuitarRevisionDto createGuitarRevisionDto);

  /// <summary>
  /// Updates a Guitar Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Guitar Revision.</param>
  /// <param name="createGuitarRevisionDto">The details of the updated Guitar.</param>
  /// <returns><see cref="GuitarRevision"/> if found; otherwise, <c>null</c></returns>
  Task<GuitarRevision?> UpdateGuitarRevisionAsync(int revisionId, CreateGuitarRevisionDto createGuitarRevisionDto);
}
