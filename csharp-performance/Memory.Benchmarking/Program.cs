using BenchmarkDotNet.Running;

namespace Memory.Benchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<GcPressureBenchmarks>();
        }
    }
}
