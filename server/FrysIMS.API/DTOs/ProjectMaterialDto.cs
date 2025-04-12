namespace FrysIMS.API.Dtos
{
    public class ProjectMaterialDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string StockName { get; set; }
        public int QuantityUsed { get; set; }
        public decimal UnitCostSnapshot { get; set; }
    }
}
