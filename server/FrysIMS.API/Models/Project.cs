using FrysIMS.API.Models;

namespace FrysIMS.API.Models
{
  public class Project
  {
    public int Id {get; set;}

    public string Name {get; set;}

    public decimal Budget {get; set;}

    public DateTime DateCreated {get; set;} = DateTime.UtcNow;

    public string CreatedByUserId {get; set;} // FK
    public ApplicationUser CreatedByUser {get; set;}

    public List<ProjectMaterial> ProjectMaterials {get; set;} = new();

  }
}
