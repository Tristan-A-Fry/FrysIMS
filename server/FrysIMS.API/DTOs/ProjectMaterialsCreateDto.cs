
namespace FrysIMS.API.Dtos
{
    public class ProjectMaterialsCreateDto
    {
        public int ProjectId { get; set; }
        public int StockId { get; set; }
        public int QuantityUsed { get; set; }
    }
}
