using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FrysIMS.API.Models;
using FrysIMS.API.Data;
using FrysIMS.API.Dtos;

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
    var locations  = await _locationService.GetAllMaterialLocationAsync();
    var dtoList = locations.Select(ml => new MaterialLocationDto
        {
          Id = ml.Id,
          Status = ml.Status,
          TimeStamp = ml.TimeStamp,
          ProjectMaterialId = ml.ProjectMaterialId,
          UpdatedByUserId = ml.UpdatedByUserId
        }).ToList();
    return Ok(dtoList);
  }

  [HttpGet("{id}", Name = "GetMaterialLocationById")]
  public async Task<IActionResult> GetMaterialLocationByIdAsync(int id)
  {
    var location = await _locationService.GetMaterialLocationByIdAsync(id);
    if(location == null) return NotFound();

    return Ok(location);
  }

  [HttpPost] 
  public async Task<IActionResult> CreateMaterialLocation([FromBody] MaterialLocationCreate dto)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
    if(string.IsNullOrEmpty(userId))
    {
      return Unauthorized("Could Not Determine User Identity");
    }

    var newLocation = new MaterialLocation
    {
      ProjectMaterialId = dto.ProjectMaterialId,
      Status = dto.Status,
      UpdatedByUserId = userId
    };


    var success = await _locationService.AddMaterialLocationAsync(newLocation);
    if(!success)
    {
      return BadRequest("Invalid ProjectMaterial ID or other error");
    }

    var responseDto = new MaterialLocationDto
    {
        Id = newLocation.Id,
        Status = newLocation.Status,
        TimeStamp = newLocation.TimeStamp,
        ProjectMaterialId = newLocation.ProjectMaterialId,
        UpdatedByUserId = newLocation.UpdatedByUserId
    };

    /*TODO: Ask more questions about this */

    return CreatedAtRoute(
        "GetMaterialLocationById",
        new { id = newLocation.Id },
        responseDto
    );
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] MaterialLocationupdateDto dto)
  {
      if(id != dto.Id)

      {
        return BadRequest("ID Mismatch");
      }

      var updated = new MaterialLocation
      {
        Id = dto.Id,
        ProjectMaterialId = dto.ProjectMaterialId,
        Status = dto.Status,
        TimeStamp = DateTime.UtcNow,
        UpdatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
      };


      var success = await _locationService.UpdateMaterialLocationAsync(updated);
      if (!success)
          return NotFound("Material location not found.");

      return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
      var success = await _locationService.DeleteMaterialLocationAsync(id);
      if (!success)
          return NotFound("Material location not found.");

      return NoContent();
  }
  [HttpGet("test/{id}")]
  public IActionResult TestRoute(int id)
  {
      return Ok(new { Message = "Matched Route!", Id = id });
  }
} 
