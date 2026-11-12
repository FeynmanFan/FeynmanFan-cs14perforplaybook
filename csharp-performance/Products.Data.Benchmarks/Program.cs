using BenchmarkDotNet.Running;
using Products.Data.Benchmarks;

namespace Products.Data.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string Token = "ghp_1234567890abcdefghijklmnopqrstuvwxyz12";

            BenchmarkRunner.Run<ProductQueryBenchmarks>();
        }
    }
}