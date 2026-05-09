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
  public async Task<IActionResult> GetGuitars([FromQuery] GuitarParams guitarParams)
    => Ok(await guitarsService.GetGuitarsAsync(guitarParams, Response));

  [AllowAnonymous]
  [HttpGet("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetGuitar(int id)
    => (await guitarsService.GetGuitarAsync(id)).ToActionResult();

  [Authorize]
  [HttpPost("create")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateInitialGuitarRevision(CreateGuitarRevisionDto createGuitarRevisionDto)
    => (await guitarsService.CreateInitialGuitarRevisionAsync(createGuitarRevisionDto, User)).ToActionResult();

  [Authorize]
  [HttpPost("{id:int}/create-revision")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateGuitarRevision(int id, CreateGuitarRevisionDto createGuitarRevision)
    => (await guitarsService.CreateGuitarRevisionAsync(id, createGuitarRevision, User)).ToActionResult();

  [Authorize(Policy = "RequireAdminRole")]
  [HttpDelete("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> DeleteGuitar(int id)
    => (await guitarsService.DeleteGuitarAsync(id)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{guitarId:int}/revisions")] // Getting all excerpts for pending update guitars
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForGuitar([FromQuery] RevisionParams revisionExcerptParams, int guitarId)
    => (await guitarsService.GetGuitarRevisionExcerptsAsync(revisionExcerptParams, guitarId, Response, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpGet("revisions")] // Getting all excerpts for pending new guitars
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerpts([FromQuery] RevisionParams revisionExcerptParams)
    => (await guitarsService.GetNewGuitarRevisionExcerptsAsync(revisionExcerptParams, Response)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("revisions/{revisionId:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRevisionSnapshot(int revisionId)
    => (await guitarsService.GetGuitarRevisionSnapshotAsync(revisionId, User)).ToActionResult();

  [AllowAnonymous]
  [HttpPatch("revisions/{revisionId:int}/edit")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(403)]
  public async Task<IActionResult> UpdateGuitarRevision(int revisionId, CreateGuitarRevisionDto createGuitarRevision)
=> (await guitarsService.UpdateGuitarRevisionAsync(revisionId, createGuitarRevision, User)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{guitarId:int}/revisions/diff")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)] // If one of the revisionIds dont exist
  public async Task<IActionResult> GetRevisionDiff([FromQuery] int revisionId1, [FromQuery] int revisionId2, int guitarId)
    => (await guitarsService.GetGuitarRevisionDiffAsync(revisionId1, revisionId2, guitarId, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpPost("revisions/{revisionId:int}/approve")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> ApproveCreateGuitarRevision(int revisionId)
    => (await guitarsService.ApproveCreateGuitarRevisionAsync(revisionId, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(500)]
  [HttpPost("{guitarId:int}/revisions/{revisionId:int}/approve")]  // update guitar
  public async Task<IActionResult> ApproveUpdateGuitarRevision(int guitarId, int revisionId)
    => (await guitarsService.ApproveUpdateGuitarRevisionAsync(guitarId, revisionId, User)).ToActionResult();
}
