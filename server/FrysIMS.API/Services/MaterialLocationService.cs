using FrysIMS.API.Models;
using FrysIMS.API.Data;
using Microsoft.EntityFrameworkCore;

public interface IMaterialLocationService 
{
  Task<List<MaterialLocation>> GetAllMaterialLocationAsync();
  Task<MaterialLocation> GetMaterialLocationByIdAsync(int id);
  Task<bool> AddMaterialLocationAsync(MaterialLocation materialLocation);
  Task<bool> UpdateMaterialLocationAsync(MaterialLocation materialLocation);
  Task<bool> DeleteMaterialLocationAsync(int id);
}

public class MaterialLocationService: IMaterialLocationService
{
  private readonly IMaterialLocationRepository _repository;
  private readonly ApplicationDbContext _context;

  public MaterialLocationService(IMaterialLocationRepository repository, ApplicationDbContext context)
  {
    _repository = repository;
    _context = context;
  } 

  public async Task<List<MaterialLocation>> GetAllMaterialLocationAsync()
  {
    return await _repository.GetAllAsync();
  }

  public async Task<MaterialLocation> GetMaterialLocationByIdAsync(int id)
  {
    return await _repository.GetByIdAsync(id);
  }

  public async Task<bool> AddMaterialLocationAsync(MaterialLocation materialLocation)
  {
    var projectMaterial = await _context.ProjectMaterials
        .FirstOrDefaultAsync(pm => pm.Id == materialLocation.ProjectMaterialId);

    if(projectMaterial == null)
    {
      Console.WriteLine("Invalid ProjectMaterial ID");
      return false;
    }

    await _repository.AddAsync(materialLocation);
    return true;

  }

  public async Task<bool> UpdateMaterialLocationAsync(MaterialLocation materialLocation)
  {
    var existing = await _repository.GetByIdAsync(materialLocation.Id);

    if(existing == null) return false;

    existing.Status = materialLocation.Status;
    existing.TimeStamp = DateTime.UtcNow;
    await _repository.UpdateAsync(existing);

    return true;
  }

  public async Task<bool> DeleteMaterialLocationAsync(int id)
  {
    var materialLocation = await _repository.GetByIdAsync(id);

    if(materialLocation == null) return false;

    await _repository.DeleteAsync(materialLocation);
    return true;
  }
}
