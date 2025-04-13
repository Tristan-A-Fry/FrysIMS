namespace FrysIMS.API.Dtos
{
  public class MaterialLocationCreate
  {
    public int ProjectMaterialId{get; set;}
    public string Status {get; set;} = "Pending Dispatch";
  }
}
