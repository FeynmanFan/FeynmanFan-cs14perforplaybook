using BenchmarkDotNet.Attributes;

namespace Compute.Benchmarking
{
    [MemoryDiagnoser(displayGenColumns: true)]
    public class BranchPredictionBenchmarks
    {
        private const int ArraySize = 10_000_000;
        private readonly int[] unsortedData = new int[ArraySize];
        private readonly Random rnd = new Random(42);

        private int[] sortedData;  // Populated in GlobalSetup

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < unsortedData.Length; i++)
                unsortedData[i] = rnd.Next(100); // random values 0-99

            sortedData = new int[unsortedData.Length];
            Array.Copy(unsortedData, sortedData, unsortedData.Length);
            Array.Sort(sortedData);  // Sort once outside timed code
        }

        [Benchmark(Baseline = true)]
        public long UnpredictableBranch()
        {
            long sum = 0;
            for (int i = 0; i < unsortedData.Length; i++)
            {
                if (unsortedData[i] > 50)  // unpredictable
                    sum += unsortedData[i] * 2;
                else
                    sum += unsortedData[i];
            }
            return sum;
        }

        [Benchmark]
        public long PredictableBranch()
        {
            long sum = 0;
            for (int i = 0; i < sortedData.Length; i++)
            {
                if (sortedData[i] > 50)    // predictable after sort
                    sum += sortedData[i] * 2;
                else
                    sum += sortedData[i];
            }
            return sum;
        }
    }
}