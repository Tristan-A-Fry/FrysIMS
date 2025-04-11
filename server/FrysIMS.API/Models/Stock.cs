
namespace FrysIMS.API.Models
{
  public class Stock
  {
    public int Id {get; set;}

    public string Name {get; set;} = string.Empty;

    public int Quantity {get; set;}

    public string Unit {get; set;} = "unit";

    public decimal OriginalPrice {get; set;} // per unit 

    public string? CreatedByUserId {get; set;} // Foreign Key -> ApplicationUser.Id

    public ApplicationUser? CreatedByUser {get; set;}
  }
}
