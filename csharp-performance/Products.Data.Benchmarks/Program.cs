using BenchmarkDotNet.Running;
using Products.Data.Benchmarks;

namespace Products.Data.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string pgp_private_key = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+/abcdefghijklmnopqrstuvwxyz==";

            BenchmarkRunner.Run<ProductQueryBenchmarks>();
        }
    }
}