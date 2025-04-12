using FrysIMS.API.Models;
using FrysIMS.API.Data;

public interface IProjectMaterialService
{
  Task<List<ProjectMaterial>> GetAllProjectMaterialAsync();
  Task<ProjectMaterial> GetProjectMaterialByIdAsync(int id);
  Task<bool> AddProjectMaterialAsync(ProjectMaterial material);
  Task<bool> UpdateProjectMaterialAsync(ProjectMaterial material);
  Task<bool> DeleteProjectMaterialAsync(int id);
}

public class ProjectMaterialService : IProjectMaterialService
{
  private readonly IProjectMaterialRepository _repository;
  private readonly ApplicationDbContext _context;

  public ProjectMaterialService(IProjectMaterialRepository repository, ApplicationDbContext context)
  {
    _repository = repository;
    _context = context;
  } 

  public async Task<List<ProjectMaterial>> GetAllProjectMaterialAsync()
  {
    return await _repository.GetAllAsync();
  }

  public async Task<ProjectMaterial> GetProjectMaterialByIdAsync(int id)
  {
    return await _repository.GetByIdAsync(id);
  }

  public async Task<bool> AddProjectMaterialAsync(ProjectMaterial material)
  {
    var stock = await _context.Stock.FindAsync(material.StockId);
    var project = await _context.Projects.FindAsync(material.ProjectId);

    if(stock == null)
    {
      Console.WriteLine("Invalid Stock Id");
      return false;
    }

    if(stock.Quantity < material.QuantityUsed)
    {
      Console.WriteLine("Not enough stock available.");
      return false;
    }

    if(project == null)
    {
      Console.WriteLine("Invalid Project Id");
      return false;
    }

    stock.Quantity -= material.QuantityUsed;
    material.UnitCostSnapshot = stock.OriginalPricePerUnit;

    await _repository.AddAsync(material);
    return true;
  }

  public async Task<bool> UpdateProjectMaterialAsync(ProjectMaterial material)
  {
    var existing = await _repository.GetByIdAsync(material.Id);

    if(existing == null) return false;

    existing.QuantityUsed = material.QuantityUsed;
    existing.StockId = material.StockId;
    
    existing.ProjectId = material.ProjectId;

    await _repository.UpdateAsync(existing);
    return true;
  }

  public async Task<bool> DeleteProjectMaterialAsync(int id)
  {
    var material = await _repository.GetByIdAsync(id);

    if(material == null) return false;

    await _repository.DeleteAsync(material);
    return true;
  }
}
