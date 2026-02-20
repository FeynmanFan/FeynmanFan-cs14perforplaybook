using System.Text;

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

        public static void CreateLargeTestFile(string path, int size = 1)
        {
            long targetSizeBytes = 25_000;

            var line = "2025-02-19T12:34:56.789Z|KDFW|Jet-A|172.45|USD|gallon|SupplierX|Remarks: none\n";
            var bytes = Encoding.UTF8.GetBytes(line);

            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true);

            long written = 0;
            while (written < targetSizeBytes * size)
            {
                fs.Write(bytes);
                written += bytes.Length;
            }
        }

        public static async Task CreateLargeGameAssetFileAsync(string path, long sizeInBytes)
        {
            // Simulate realistic game asset data: 64KB "resource chunks"
            var chunk = new byte[65536];
            Random.Shared.NextBytes(chunk);

            await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 131072, true);

            long written = 0;
            while (written < sizeInBytes)
            {
                int toWrite = (int)Math.Min(chunk.Length, sizeInBytes - written);
                await fs.WriteAsync(chunk.AsMemory(0, toWrite));
                written += toWrite;
            }

            Console.WriteLine("File created successfully.");
        }
    }
}
