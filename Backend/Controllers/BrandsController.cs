using Backend.Common.Params;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class BrandsController(IBrandService brandsService) : BaseApiController
{
  [AllowAnonymous]
  [HttpGet]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetAll([FromQuery] BrandSearchParams brandParams)
    => Ok(await brandsService.GetAllAsync(brandParams, Response));

  [AllowAnonymous]
  [HttpGet("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetById(int id)
    => (await brandsService.GetByIdAsync(id)).ToActionResult();

  [Authorize]
  [HttpPost("create")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateInitialRevision(BrandCreateRevisionDto createBrandRevisionDto)
    => (await brandsService.CreateInitialRevisionAsync(createBrandRevisionDto, User)).ToActionResult();

  [Authorize]
  [HttpPost("{id:int}/create-revision")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateRevision(int id, BrandCreateRevisionDto createBrandRevision)
    => (await brandsService.CreateRevisionAsync(id, createBrandRevision, User)).ToActionResult();

  [Authorize(Policy = "RequireAdminRole")]
  [HttpDelete("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> Delete(int id)
    => (await brandsService.DeleteAsync(id)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{brandId:int}/revisions")]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForPendingUpdate([FromQuery] RevisionParams revisionExcerptParams, int brandId)
    => (await brandsService.GetRevisionExcerptsAsync(revisionExcerptParams, brandId, Response, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpGet("revisions")]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerptsForPendingNew([FromQuery] RevisionParams revisionExcerptParams)
    => (await brandsService.GetNewRevisionExcerptsAsync(revisionExcerptParams, Response)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("revisions/{revisionId:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRevisionSnapshot(int revisionId)
    => (await brandsService.GetRevisionSnapshotAsync(revisionId, User)).ToActionResult();

  [AllowAnonymous]
  [HttpPatch("revisions/{revisionId:int}/edit")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(403)]
  public async Task<IActionResult> UpdateRevision(int revisionId, BrandCreateRevisionDto createBrandRevision)
=> (await brandsService.UpdateRevisionAsync(revisionId, createBrandRevision, User)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{brandId:int}/revisions/diff")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)] // If one of the revisionIds dont exist
  public async Task<IActionResult> GetRevisionDiff([FromQuery] int revisionId1, [FromQuery] int revisionId2, int brandId)
    => (await brandsService.GetRevisionDiffAsync(revisionId1, revisionId2, brandId, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [HttpPost("revisions/{revisionId:int}/approve")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> ApproveCreateRevision(int revisionId, ConcurrencyDto revisionIterationDto)
    => (await brandsService.ApproveCreateRevisionAsync(revisionId, revisionIterationDto.RowVersion, User)).ToActionResult();

  [Authorize(Policy = "RequireModeratorRole")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(409)]
  [ProducesResponseType(500)]
  [HttpPost("{brandId:int}/revisions/{revisionId:int}/approve")]  // update brand
  public async Task<IActionResult> ApproveUpdateRevision(int brandId, int revisionId, ConcurrencyDto revisionIterationDto)
    => (await brandsService.ApproveUpdateRevisionAsync(brandId, revisionId, revisionIterationDto.RowVersion, User)).ToActionResult();
}
