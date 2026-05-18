using Backend.Common.Params;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class GuitarsController(IGuitarService guitarsService) : BaseApiController
{
  [AllowAnonymous]
  [HttpGet]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetAll([FromQuery] GuitarSearchParams guitarParams)
    => Ok(await guitarsService.GetAllAsync(guitarParams, Response));

  [AllowAnonymous]
  [HttpGet("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetById(int id)
    => (await guitarsService.GetByIdAsync(id)).ToActionResult();

  [Authorize]
  [HttpPost("create")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateInitialRevision(GuitarCreateRevisionDto createGuitarRevisionDto)
    => (await guitarsService.CreateInitialRevisionAsync(createGuitarRevisionDto, User)).ToActionResult();

  [Authorize]
  [HttpPost("{id:int}/create-revision")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateRevision(int id, GuitarCreateRevisionDto createGuitarRevision)
    => (await guitarsService.CreateRevisionAsync(id, createGuitarRevision, User)).ToActionResult();

  [Authorize(Policy = "RequireAdminRole")]
  [HttpDelete("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> Delete(int id)
    => (await guitarsService.DeleteAsync(id)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{guitarId:int}/revisions")]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForPendingUpdate([FromQuery] RevisionParams revisionExcerptParams, int guitarId)
    => (await guitarsService.GetRevisionExcerptsAsync(revisionExcerptParams, guitarId, Response, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpGet("revisions")]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForPendingNew([FromQuery] RevisionParams revisionExcerptParams)
    => (await guitarsService.GetNewRevisionExcerptsAsync(revisionExcerptParams, Response)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("revisions/{revisionId:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRevisionSnapshot(int revisionId)
    => (await guitarsService.GetRevisionSnapshotAsync(revisionId, User)).ToActionResult();

  [AllowAnonymous]
  [HttpPatch("revisions/{revisionId:int}/edit")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(403)]
  public async Task<IActionResult> UpdateRevision(int revisionId, GuitarCreateRevisionDto createGuitarRevision)
=> (await guitarsService.UpdateRevisionAsync(revisionId, createGuitarRevision, User)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{guitarId:int}/revisions/diff")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)] // If one of the revisionIds dont exist
  public async Task<IActionResult> GetRevisionDiff([FromQuery] int revisionId1, [FromQuery] int revisionId2, int guitarId)
    => (await guitarsService.GetRevisionDiffAsync(revisionId1, revisionId2, guitarId, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpPost("revisions/{revisionId:int}/approve")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> ApproveCreateRevision(int revisionId, ConcurrencyDto revisionIterationDto)
    => (await guitarsService.ApproveCreateRevisionAsync(revisionId, revisionIterationDto.RowVersion, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  [HttpPost("{guitarId:int}/revisions/{revisionId:int}/approve")]  // update guitar
  public async Task<IActionResult> ApproveUpdateRevision(int guitarId, int revisionId, ConcurrencyDto revisionIterationDto)
    => (await guitarsService.ApproveUpdateRevisionAsync(guitarId, revisionId, revisionIterationDto.RowVersion, User)).ToActionResult();
}
