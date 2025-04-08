namespace FrysIMS.API.Models
{
  public class MaterialLocation
  {
    public int Id {get; set;}

    public int ProjectMaterialId {get; set;} //Fk
    public ProjectMaterial ProjectMaterial {get; set;}

    public string Status {get; set;} = "In Transit";

    public DateTime TimeStamp {get; set;} = DateTime.UtcNow;

    public string UpdatedByUserId {get; set;} //Fk
    public ApplicationUser UpdatedByUser {get; set;}
  }
}
