using BenchmarkDotNet.Attributes;

namespace Compute.Benchmarking
{
    [ShortRunJob]
    public class CacheMissBenchmarks
    {
        private const int ArraySize = 16 * 1024 * 1024; // 16 MB array (larger than L3 cache on most CPUs)
        private readonly int[] data = new int[ArraySize];
        private readonly Random rnd = new Random(42);

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = i;
        }

        [Benchmark(Baseline = true)]
        public long RandomAccess()          // ← Terrible cache behavior
        {
            long sum = 0;
            for (int i = 0; i < ArraySize; i++)
            {   
                int index = rnd.Next(ArraySize);        // random jump → cache misses
                sum += data[index];
            }
            return sum;
        }

        [Benchmark]
        public long SequentialAccess()      // ← Excellent cache behavior
        {
            long sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                int index = rnd.Next(ArraySize);        // to compare apples to apples
                sum += data[i];                         // sequential → prefetcher loves this
            }
            return sum;
        }
    }
}