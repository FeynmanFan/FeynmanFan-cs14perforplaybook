using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Products.Data.Benchmarks
{
    [MemoryDiagnoser]
    [ShortRunJob]
    public class ProductQueryBenchmarks
    {
        private ProductDbContext _context = null!;

        [GlobalSetup]
        public void Setup()
        {
            var connectionString =
            "Server=localhost,1433;" +
            "Database=ProductPlayground;" +
            "User Id=sa;" +
            "Password=c#p3rfpl4y;" +
            "TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            _context = new ProductDbContext(options);
        }

        [Benchmark(Baseline = true)]
        public int LateEvaluation_ServerSideFilter()
        {
            // Most efficient: filter on server, then materialize
            return _context.Products
                .Where(p => p.IsDiscounted)
                .Count();
        }

        [Benchmark]
        public int EarlyEvaluation_ClientSideFilter()
        {
            // Forces early evaluation → pulls ALL products into memory first
            var allProducts = _context.Products.ToList();

            return allProducts
                .Count(p => p.IsDiscounted);
        }
    }
}