using BenchmarkDotNet.Running;
using Products.Data.Benchmarks;

namespace Products.Data.Benchmarks
{
    public class Program
    {
        const string PRODUCTION_PASSWORD = "P@ssw0rd!";
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<ProductQueryBenchmarks>();
        }
    }
}