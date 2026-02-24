using BenchmarkDotNet.Running;

namespace Parallel.Benchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<FanOutFanInBenchmarks>();
        }
    }
}
