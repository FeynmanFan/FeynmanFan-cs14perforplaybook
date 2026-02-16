namespace performance_helper
{
    public class DataHelper()
    {
        public static IEnumerable<Product> CreateProductsCollection(int count)
        {
            var random = new Random(2112); // reproducible

            var categories = new[]
            {
                "Electronics", "Books", "Clothing", "Home & Kitchen", "Sports",
                "Toys", "Beauty", "Automotive", "Groceries", "Office Supplies"
            };

            var namePrefixes = new[] { "Awesome", "Pro", "Ultra", "Smart", "Elite", "Classic", "Premium", "Deluxe", "Basic", "Max" };
            var productTypes = new[] { "Phone", "Laptop", "Shirt", "Book", "Headphones", "Blender", "Yoga Mat", "Puzzle", "Lipstick", "Tire" };

            var products = new List<Product>(count);

            for (int i = 0; i < count; i++)
            {
                string id = $"P{i}";

                // name not guaranteed unique, but probably for count values < 100k
                string name = $"{namePrefixes[random.Next(namePrefixes.Length)]} {productTypes[random.Next(productTypes.Length)]} {random.Next(100, 1000)}";
                double price = Math.Round(Math.Exp(random.NextDouble() * 3 + 1.5) + 5, 2); // log-normal-ish distribution
                int quantity = random.Next(0, 201);
                bool inStock = random.NextDouble() < 0.85;
                string category = categories[random.Next(categories.Length)];

                products.Add(new Product { Name = name, Category = category, Price = price, Id = id, InStock = inStock});
            }

            return products;
        }
    }
}
