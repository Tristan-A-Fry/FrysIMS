
using FrysIMS.API.Data;
using FrysIMS.API.Models;
using Microsoft.EntityFrameworkCore;


public interface IProjectMaterialRepository
{
  Task<List<ProjectMaterial>> GetAllAsync();
  Task<ProjectMaterial> GetByIdAsync(int id);
  Task AddAsync(ProjectMaterial material); 
  Task UpdateAsync(ProjectMaterial material);
  Task DeleteAsync(ProjectMaterial material);
}

public class ProjectMaterialRepository : IProjectMaterialRepository
{
  private readonly ApplicationDbContext _context;

  public ProjectMaterialRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<List<ProjectMaterial>> GetAllAsync()=> await _context.ProjectMaterials.ToListAsync();

  public async Task<ProjectMaterial> GetByIdAsync(int id)=> await _context.ProjectMaterials.FindAsync(id);

  public async Task AddAsync(ProjectMaterial material)
  {
    _context.ProjectMaterials.Add(material);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateAsync(ProjectMaterial material)
  {
    _context.ProjectMaterials.Update(material);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(ProjectMaterial material)
  {
    _context.ProjectMaterials.Remove(material);
    await _context.SaveChangesAsync();
  }
}
