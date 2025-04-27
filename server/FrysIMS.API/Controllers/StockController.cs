using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FrysIMS.API.Models;
using FrysIMS.API.Dtos;

[ApiController]
[Route("api/stock")]
// [Authorize(Roles = "Admin,ProjectManager")]
public class StockController : ControllerBase
{

  private readonly IStockService _stockService;

  public StockController(IStockService stockService)
  {
    _stockService = stockService;
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> GetAllStock()
  {
    var stocks = await _stockService.GetAllStockAsync();

    var stockDtos = stocks.Select(s => new StockDto
    {
      Id = s.Id,
      Name = s.Name,
      Quantity = s.Quantity,
      Unit = s.Unit,
      OriginalPricePerUnit = s.OriginalPricePerUnit,
      CreatedByUserId = s.CreatedByUserId
    }).ToList();

    return Ok(stockDtos);
  }


  [Authorize(Roles = "Admin,InventorySpecialist")]
  [HttpGet("id")]
  public async Task<IActionResult> GetStockById(int id)
  {
    var stock = await _stockService.GetStockByIdAsync(id);
    if(stock == null) return NotFound();
    return Ok(stock);
  }

  [Authorize(Roles = "Admin,InventorySpecialist")]
  [HttpPost]
  public async Task<IActionResult> CreateStock([FromBody] StockCreateDto dto)
  {
    // var userId = User.FindFirst("sub")?.Value; 
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if(string.IsNullOrEmpty(userId))
    {
      return Unauthorized("Could not determine user identity.");
    }

    var stock = new Stock
    {
      Name = dto.Name,
      Quantity = dto.Quantity,
      Unit = dto.Unit,
      OriginalPricePerUnit = dto.OriginalPricePerUnit,
      CreatedByUserId = userId
    };


    var success = await _stockService.AddStockAsync(stock);
    if(!success)
    {
      return BadRequest("A stock item with that name already exists.");
    }

    return CreatedAtAction(nameof(GetStockById), new {id = stock.Id} , dto);
  }


  [Authorize(Roles = "Admin,InventorySpecialist")]
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateDto dto)
  {
    if (id != dto.Id) return BadRequest("ID mismatch");

    var stock = await _stockService.GetStockByIdAsync(id);

    if(stock == null) return NotFound();

    stock.Name = dto.Name;
    stock.Quantity = dto.Quantity;
    stock.Unit = dto.Unit;
    stock.OriginalPricePerUnit = dto.OriginalPricePerUnit;

    var success = await _stockService.UpdateStockAsync(stock);
    if(!success) return NotFound();

    return NoContent();
  }

  [Authorize(Roles = "Admin,InventorySpecialist")]
  [HttpDelete("{id}")] 
  public async Task<IActionResult> DeleteStock(int id)
  {
    var success = await _stockService.DeleteStockAsync(id);
    if(!success) return NotFound();

    return NoContent();
  }
}
