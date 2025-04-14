
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FrysIMS.API.Models;
using FrysIMS.API.Data;
using FrysIMS.API.Dtos;

[ApiController]
[Route("api/projectMaterial")]
[Authorize(Roles = "Admin,ProjectManager")]
public class ProjectMaterialController : ControllerBase
{

  private readonly IProjectMaterialService _materialService;
  private readonly ApplicationDbContext _context;

  public ProjectMaterialController(IProjectMaterialService materialService, ApplicationDbContext context)
  {
    _materialService = materialService;
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> GetAllProjectMaterial()
  {
    var materials = await _materialService.GetAllProjectMaterialAsync();
    var materialDto = materials.Select(pm => new ProjectMaterialDto
        {
          Id = pm.Id,
          ProjectName = pm.Project?.Name??"Unknown",
          StockName = pm.Stock?.Name??"Unknown",
          QuantityUsed = pm.QuantityUsed,
          UnitCostSnapshot = pm.UnitCostSnapshot
        }).ToList();

    return Ok(materialDto);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetProjectMaterialById(int id)
  {
    var material = await _materialService.GetProjectMaterialByIdAsync(id);
    if(material == null) return NotFound();
    return Ok(material);
  }

  [HttpPost]
  public async Task<IActionResult> CreateProjectMaterial([FromBody] ProjectMaterialsCreateDto dto)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
        return Unauthorized("Could not determine user identity.");

    var project = await _context.Projects.FindAsync(dto.ProjectId);

    if (project == null)
        return NotFound("Project does not exist.");

    if (project.CreatedByUserId != userId)
        return Forbid("You are not the owner of this project.");

    var projectMaterial = new ProjectMaterial 
    {
      ProjectId = dto.ProjectId,
      StockId = dto.StockId,
      QuantityUsed = dto.QuantityUsed
    };


    // ✅ Proceed with adding the project material
    var success = await _materialService.AddProjectMaterialAsync(projectMaterial);
    if (!success)
        return BadRequest("Failed to add project material. Check StockId or ProjectId.");

    return CreatedAtAction(nameof(GetProjectMaterialById), new {id = projectMaterial.Id} , dto);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateProjectMaterial(int id, [FromBody] ProjectMaterial material)
  {
    if (id != material.Id) return BadRequest("ID mismatch");

    var success = await _materialService.UpdateProjectMaterialAsync(material);
    if(!success) return NotFound();

    return NoContent();
  }

  [HttpDelete("{id}")] 
  public async Task<IActionResult> DeleteProjectMaterial(int id)
  {
    var success = await _materialService.DeleteProjectMaterialAsync(id);
    if(!success) return NotFound();

    return NoContent();
  }
  [HttpGet("project/{projectId}")]
  public async Task<IActionResult> GetMaterialsByProjectId(int projectId)
  {
      var materials = await _materialService.GetAllProjectMaterialAsync();

      var filtered = materials
          .Where(pm => pm.ProjectId == projectId)
          .Select(pm => new ProjectMaterialDto
          {
              Id = pm.Id,
              ProjectName = pm.Project?.Name ?? "Unknown",
              StockName = pm.Stock?.Name ?? "Unknown",
              QuantityUsed = pm.QuantityUsed,
              UnitCostSnapshot = pm.UnitCostSnapshot
          })
          .ToList();

      return Ok(filtered);
  }
}
