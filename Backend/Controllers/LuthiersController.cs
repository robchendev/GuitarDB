using Backend.Common.Params;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class LuthiersController(ILuthierService luthiersService) : BaseApiController
{
  [AllowAnonymous]
  [HttpGet]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetAll([FromQuery] LuthierSearchParams luthierParams)
    => Ok(await luthiersService.GetAllAsync(luthierParams, Response));

  [AllowAnonymous]
  [HttpGet("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetById(int id)
    => (await luthiersService.GetByIdAsync(id)).ToActionResult();

  [Authorize]
  [HttpPost("create")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateInitialRevision(LuthierCreateRevisionDto createLuthierRevisionDto)
    => (await luthiersService.CreateInitialRevisionAsync(createLuthierRevisionDto, User)).ToActionResult();

  [Authorize]
  [HttpPost("{id:int}/create-revision")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateRevision(int id, LuthierCreateRevisionDto createLuthierRevision)
    => (await luthiersService.CreateRevisionAsync(id, createLuthierRevision, User)).ToActionResult();

  [Authorize(Policy = "RequireAdminRole")]
  [HttpDelete("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> Delete(int id)
    => (await luthiersService.DeleteAsync(id)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{luthierId:int}/revisions")]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForPendingUpdates([FromQuery] RevisionParams revisionExcerptParams, int luthierId)
    => (await luthiersService.GetRevisionExcerptsAsync(revisionExcerptParams, luthierId, Response, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpGet("revisions")]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForPendingNew([FromQuery] RevisionParams revisionExcerptParams)
    => (await luthiersService.GetNewRevisionExcerptsAsync(revisionExcerptParams, Response)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("revisions/{revisionId:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRevisionSnapshot(int revisionId)
    => (await luthiersService.GetRevisionSnapshotAsync(revisionId, User)).ToActionResult();

  [AllowAnonymous]
  [HttpPatch("revisions/{revisionId:int}/edit")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(403)]
  public async Task<IActionResult> UpdateRevision(int revisionId, LuthierCreateRevisionDto createLuthierRevision)
    => (await luthiersService.UpdateRevisionAsync(revisionId, createLuthierRevision, User)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{luthierId:int}/revisions/diff")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  public async Task<IActionResult> GetRevisionDiff([FromQuery] int revisionId1, [FromQuery] int revisionId2, int luthierId)
    => (await luthiersService.GetRevisionDiffAsync(revisionId1, revisionId2, luthierId, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpPost("revisions/{revisionId:int}/approve")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> ApproveCreateRevision(int revisionId, ConcurrencyDto revisionIterationDto)
    => (await luthiersService.ApproveCreateRevisionAsync(revisionId, revisionIterationDto.RowVersion, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  [HttpPost("{luthierId:int}/revisions/{revisionId:int}/approve")]  // update luthier
  public async Task<IActionResult> ApproveUpdateRevision(int luthierId, int revisionId, ConcurrencyDto revisionIterationDto)
    => (await luthiersService.ApproveUpdateRevisionAsync(luthierId, revisionId, revisionIterationDto.RowVersion, User)).ToActionResult();
}
