using FrysIMS.API.Models;
using FrysIMS.API.Data;
using Microsoft.EntityFrameworkCore;
using FrysIMS.API.Dtos;

public interface IProjectService
{
  Task<List<ProjectDto>> GetAllProjectAsync();
  Task<Project> GetProjectByIdAsync(int id);
  Task<bool> AddProjectAsync(Project project, string userId);
  Task<bool> UpdateProjectAsync(Project project, string userId);
  Task<bool> DeleteProjectAsync(int id, string userId);
}

public class ProjectService : IProjectService
{
  private readonly IProjectRepository _repository;
  private readonly ApplicationDbContext _context;

  public ProjectService(IProjectRepository repository, ApplicationDbContext context)
  {
    _repository = repository;
    _context = context;
  } 

  public async Task<List<ProjectDto>> GetAllProjectAsync()
  {
    // return await _repository.GetAllAsync();
    var projects = await _context.Projects
        .Include(p => p.ProjectMaterials)
        .ThenInclude(pm => pm.Stock)
        .ToListAsync();

    return projects.Select(p => new ProjectDto
    {
        Id = p.Id,
        Name = p.Name,
        Budget = p.Budget,
        CreatedByUserId = p.CreatedByUserId,
        DateCreated = p.DateCreated,
        ProjectMaterials = p.ProjectMaterials.Select(pm => new ProjectMaterialDto
        {
            Id = pm.Id,
            ProjectName = pm.Project?.Name ?? "Unknown",
            StockName = pm.Stock?.Name ?? "Unknown",
            QuantityUsed = pm.QuantityUsed,
            UnitCostSnapshot = pm.UnitCostSnapshot
        }).ToList()
    }).ToList();
  }

  public async Task<Project> GetProjectByIdAsync(int id)
  {
    return await _repository.GetByIdAsync(id);
  }

  public async Task<bool> AddProjectAsync(Project project, string userId)
  {
    var existing = await _repository.GetAllAsync();
    if(existing.Any(s => s.Name.ToLower() == project.Name.ToLower()))
    {
      Console.WriteLine("Duplicate project name attempted.");
      return false;
    }
    project.CreatedByUserId = userId;
    project.DateCreated = DateTime.UtcNow;


    await _repository.AddAsync(project);
    return true;
  }

  public async Task<bool> UpdateProjectAsync(Project project, string userId)
  {
    var existing = await _repository.GetByIdAsync(project.Id);

    if(project == null || project.CreatedByUserId != userId)
    {
      return false;
    }

    existing.Name = project.Name;
    existing.Budget = project.Budget;

    await _repository.UpdateAsync(existing);
    return true;
  }

  public async Task<bool> DeleteProjectAsync(int id, string userId)
  {
    var project = await _repository.GetByIdAsync(id);
    if(project == null || project.CreatedByUserId != userId)
    {
      return false;
    }

    await _repository.DeleteAsync(project);
    return true;
  }
}
