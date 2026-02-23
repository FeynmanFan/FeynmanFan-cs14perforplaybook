using BenchmarkDotNet.Running;

namespace Compute.Benchmarking
{
    class Program
    {
        static void Main() => BenchmarkRunner.Run(new[]
        {
            //BenchmarkConverter.TypeToBenchmarks(typeof(CacheMissBenchmarks)),
            // BenchmarkConverter.TypeToBenchmarks(typeof(BranchPredictionBenchmarks)),
            BenchmarkConverter.TypeToBenchmarks(typeof(ScalarVsVectorBenchmarks))
        });
    }
}
