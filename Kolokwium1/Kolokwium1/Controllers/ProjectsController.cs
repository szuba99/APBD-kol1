using Kolokwium1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IDbService _dbService;

    public ProjectsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectDetails(int id)
    {
        try
        {
            var project = await _dbService.GetProjectDetailsAsync(id);
            return Ok(project);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}