using BenchmarkDotNet.Running;
using Products.Data.Benchmarks;

namespace Products.Data.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string ClientSecret = "ThisIsAFakeAzureADClientSecret1234567890AbCdEfGhIjKlMnOpQrStUvWxYz";

            BenchmarkRunner.Run<ProductQueryBenchmarks>();
        }
    }
}