
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FrysIMS.API.Models;

[ApiController]
[Route("api/projects")]
[Authorize(Roles = "Admin,ProjectManager")] 
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [AllowAnonymous] 
    public async Task<IActionResult> GetAll()
    {
        var projects = await _projectService.GetAllProjectAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound("Project not found.");

        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Project project)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var success = await _projectService.AddProjectAsync(project, userId);
        if (!success)
            return BadRequest("Project name already exists.");

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Project project)
    {
        if (id != project.Id)
            return BadRequest("ID mismatch.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var success = await _projectService.UpdateProjectAsync(project, userId);
        if (!success)
            return Forbid("You do not have permission to modify this project.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var success = await _projectService.DeleteProjectAsync(id, userId);
        if (!success)
            return Forbid("You do not have permission to delete this project.");

        return NoContent();
    }
}
