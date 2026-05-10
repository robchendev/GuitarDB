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
  public async Task<IActionResult> GetLuthiers([FromQuery] LuthierParams luthierParams)
    => Ok(await luthiersService.GetLuthiersAsync(luthierParams, Response));

  [AllowAnonymous]
  [HttpGet("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetLuthier(int id)
    => (await luthiersService.GetLuthierAsync(id)).ToActionResult();

  [Authorize]
  [HttpPost("create")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateInitialLuthierRevision(CreateLuthierRevisionDto createLuthierRevisionDto)
    => (await luthiersService.CreateInitialLuthierRevisionAsync(createLuthierRevisionDto, User)).ToActionResult();

  [Authorize]
  [HttpPost("{id:int}/create-revision")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateLuthierRevision(int id, CreateLuthierRevisionDto createLuthierRevision)
    => (await luthiersService.CreateLuthierRevisionAsync(id, createLuthierRevision, User)).ToActionResult();

  [Authorize(Policy = "RequireAdminRole")]
  [HttpDelete("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> DeleteLuthier(int id)
    => (await luthiersService.DeleteLuthierAsync(id)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{luthierId:int}/revisions")] // Getting all excerpts for pending update luthiers
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForLuthier([FromQuery] RevisionParams revisionExcerptParams, int luthierId)
    => (await luthiersService.GetLuthierRevisionExcerptsAsync(revisionExcerptParams, luthierId, Response, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpGet("revisions")] // Getting all excerpts for pending new luthiers
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerpts([FromQuery] RevisionParams revisionExcerptParams)
    => (await luthiersService.GetNewLuthierRevisionExcerptsAsync(revisionExcerptParams, Response)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("revisions/{revisionId:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRevisionSnapshot(int revisionId)
    => (await luthiersService.GetLuthierRevisionSnapshotAsync(revisionId, User)).ToActionResult();

  [AllowAnonymous]
  [HttpPatch("revisions/{revisionId:int}/edit")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(403)]
  public async Task<IActionResult> UpdateLuthierRevision(int revisionId, CreateLuthierRevisionDto createLuthierRevision)
=> (await luthiersService.UpdateLuthierRevisionAsync(revisionId, createLuthierRevision, User)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{luthierId:int}/revisions/diff")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)] // If one of the revisionIds dont exist
  public async Task<IActionResult> GetRevisionDiff([FromQuery] int revisionId1, [FromQuery] int revisionId2, int luthierId)
    => (await luthiersService.GetLuthierRevisionDiffAsync(revisionId1, revisionId2, luthierId, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpPost("revisions/{revisionId:int}/approve")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> ApproveCreateLuthierRevision(int revisionId, RevisionIterationDto revisionIterationDto)
    => (await luthiersService.ApproveCreateLuthierRevisionAsync(revisionId, revisionIterationDto.Iteration, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  [HttpPost("{luthierId:int}/revisions/{revisionId:int}/approve")]  // update luthier
  public async Task<IActionResult> ApproveUpdateLuthierRevision(int luthierId, int revisionId, RevisionIterationDto revisionIterationDto)
    => (await luthiersService.ApproveUpdateLuthierRevisionAsync(luthierId, revisionId, revisionIterationDto.Iteration, User)).ToActionResult();
}
