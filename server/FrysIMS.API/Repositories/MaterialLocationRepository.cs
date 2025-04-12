using FrysIMS.API.Data;
using FrysIMS.API.Models;
using Microsoft.EntityFrameworkCore;

public interface IMaterialLocationRepository
{
  Task<List<MaterialLocation>> GetAllAsync();
  Task<MaterialLocation> GetByIdAsync(int id);
  Task AddAsync(MaterialLocation materialLocation); 
  Task UpdateAsync(MaterialLocation materialLocation);
  Task DeleteAsync(MaterialLocation materialLocation);
}

  public class MaterialLocationRepository : IMaterialLocationRepository
  {
    private readonly ApplicationDbContext _context;

    public MaterialLocationRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<MaterialLocation>> GetAllAsync()=> await _context.MaterialLocations.ToListAsync();

    public async Task<MaterialLocation> GetByIdAsync(int id)=> await _context.MaterialLocations.FindAsync(id);

    public async Task AddAsync(MaterialLocation materialLocation)
    {
      _context.MaterialLocations.Add(materialLocation);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MaterialLocation materialLocation)
    {
      _context.MaterialLocations.Update(materialLocation);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MaterialLocation materialLocation)
    {
      _context.MaterialLocations.Remove(materialLocation);
      await _context.SaveChangesAsync();
    }
}

