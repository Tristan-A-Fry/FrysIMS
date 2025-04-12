using FrysIMS.API.Models;

public interface IProjectService
{
  Task<List<Project>> GetAllProjectAsync();
  Task<Project> GetProjectByIdAsync(int id);
  Task<bool> AddProjectAsync(Project project, string userId);
  Task<bool> UpdateProjectAsync(Project project, string userId);
  Task<bool> DeleteProjectAsync(int id, string userId);
}

public class ProjectService : IProjectService
{
  private readonly IProjectRepository _repository;

  public ProjectService(IProjectRepository repository)
  {
    _repository = repository;
  } 

  public async Task<List<Project>> GetAllProjectAsync()
  {
    return await _repository.GetAllAsync();
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
