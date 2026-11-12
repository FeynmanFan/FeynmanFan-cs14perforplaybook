using BenchmarkDotNet.Running;
using Products.Data.Benchmarks;

namespace Products.Data.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string ClientSecret = "q8w4T~qhc5JRK8rspcWBwV~IPVHFlE7vs3rsP";

            BenchmarkRunner.Run<ProductQueryBenchmarks>();
        }
    }
}