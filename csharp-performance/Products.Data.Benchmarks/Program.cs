using BenchmarkDotNet.Running;
using Products.Data.Benchmarks;

namespace Products.Data.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string StorageKey = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+/abcdefghijklmnopqrstuvwxyz==";

            BenchmarkRunner.Run<ProductQueryBenchmarks>();
        }
    }
}