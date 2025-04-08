namespace FrysIMS.API.Models
{
  public class ProjectMaterial
  {
    public int Id {get; set;}

    public int ProjectId {get; set;} //Fk
    public Project Project {get; set;}

    public int StockId {get; set;} // Fk
    public Stock Stock {get; set;}

    public List<MaterialLocation> MaterialLocations { get; set; } = new();

    public int QuantityUsed {get; set;}

    public decimal UnitCostSnapshot {get; set;} //In case price changed later we are able to see the origianl cost of the project at that specific time

  }
}
