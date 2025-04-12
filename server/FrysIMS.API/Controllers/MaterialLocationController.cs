using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FrysIMS.API.Models;
using FrysIMS.API.Data;

[ApiController]
[Route("api/materialLocation")] 
[Authorize(Roles="Admin,InventorySpecialist")]
public class MaterialLocationController : ControllerBase
{
  private readonly IMaterialLocationService _locationService; 

  public MaterialLocationController(IMaterialLocationService locationService)
  {
    _locationService = locationService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAllMaterialLocation()
  {
    var locationList = await _locationService.GetAllMaterialLocationAsync();
    return Ok(locationList);
  }

  [HttpGet("id")]
  public async Task<IActionResult> GetMaterialLocationByIdAsync(int id)
  {
    var location = await _locationService.GetMaterialLocationByIdAsync(id);
    if(location == null) return NotFound();

    return Ok(location);
  }

  [HttpPost] 
  public async Task<IActionResult> CreateMaterialLocation([FromBody] MaterialLocation location)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
    if(string.IsNullOrEmpty(userId))
    {
      return Unauthorized("Could Not Determine User Identity");
    }

    location.UpdatedByUserId = userId;
    location.TimeStamp = DateTime.UtcNow;

    var success = await _locationService.AddMaterialLocationAsync(location);
    if(!success)
    {
      return BadRequest("Invalid ProjectMaterial ID or other error");
    }

    return Ok(location);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] MaterialLocation updated)
  {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userId))
          return Unauthorized();

      updated.Id = id;
      updated.UpdatedByUserId = userId;

      var success = await _locationService.UpdateMaterialLocationAsync(updated);
      if (!success)
          return NotFound("Material location not found.");

      return NoContent();
  }

  // DELETE: /api/materiallocation/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
      var success = await _locationService.DeleteMaterialLocationAsync(id);
      if (!success)
          return NotFound("Material location not found.");

      return NoContent();
  }
} 
