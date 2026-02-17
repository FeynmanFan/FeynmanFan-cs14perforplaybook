using BenchmarkDotNet.Running;

namespace CatalogApp.Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
