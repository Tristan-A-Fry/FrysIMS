 namespace FrysIMS.API.Dtos
{
  public class MaterialLocationDto
  {
    public int Id {get; set;}
    public string Status {get; set;}
    public DateTime TimeStamp {get; set;}
    public int ProjectMaterialId {get; set;}
    public string UpdatedByUserId{get; set;}
  }
}
