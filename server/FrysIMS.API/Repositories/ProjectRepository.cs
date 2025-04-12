using FrysIMS.API.Data;
using FrysIMS.API.Models;
using Microsoft.EntityFrameworkCore;

public interface IProjectRepository
{
  Task<List<Project>> GetAllAsync();
  Task<Project> GetByIdAsync(int id);
  Task AddAsync(Project project); 
  Task UpdateAsync(Project project);
  Task DeleteAsync(Project project);
}

public class ProjectRepository : IProjectRepository
{
  private readonly ApplicationDbContext _context;

  public ProjectRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<List<Project>> GetAllAsync()=> await _context.Projects.ToListAsync();

  public async Task<Project> GetByIdAsync(int id)=> await _context.Projects.FindAsync(id);

  public async Task AddAsync(Project project)
  {
    _context.Projects.Add(project);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateAsync(Project project)
  {
    _context.Projects.Update(project);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(Project project)
  {
    _context.Projects.Remove(project);
    await _context.SaveChangesAsync();
  }

}
