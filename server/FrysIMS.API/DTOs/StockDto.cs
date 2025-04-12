namespace FrysIMS.API.Dtos
{
    public class StockDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal OriginalPricePerUnit { get; set; }
        public string CreatedByUserId { get; set; } // Optional: to show who added it
    }
}

