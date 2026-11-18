// Product.cs
namespace Products.Data
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsDiscounted { get; set; }
        public int StockQuantity { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}