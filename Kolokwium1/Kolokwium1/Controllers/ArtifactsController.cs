using Kolokwium1.Models.DTOs;
using Kolokwium1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1.Controllers;

[ApiController]
[Route("api/artifacts")]
public class ArtifactsController : ControllerBase
{
    private readonly IDbService _dbService;

    public ArtifactsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddArtifactWithProject([FromBody] AddArtifactWithProjectDto dto)
    {
        try
        {
            await _dbService.AddArtifactWithProjectAsync(dto);
            return Ok("Dodano artefakt i projekt");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}