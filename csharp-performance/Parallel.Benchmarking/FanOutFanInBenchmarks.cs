namespace Parallel.Benchmarking
{
    using BenchmarkDotNet.Attributes;
    using System;
    using System.Threading.Tasks;

    [ShortRunJob]
    [MemoryDiagnoser(displayGenColumns: true)]
    public class FanOutFanInBenchmarks
    {
        [Params(8_000L, 200_000L)]
        public long Size { get; set; }

        private double[] data;

        private readonly object lockObject = new object();

        [GlobalSetup]
        public void Setup()
        {
            Console.WriteLine($"Allocating array of size {Size:N0} (~{Size * 8 / 1_000_000_000.0:F1} GB)...");
            data = new double[Size];
            for (long i = 0; i < Size; i++)
                data[i] = i % 1000;   // simple values
        }

        // ===================================================================
        // 1. Serial Execution (Baseline)
        // ===================================================================
        [Benchmark(Baseline = true)]
        public double Serial()
        {
            double sum = 0.0;
            for (long i = 0; i < Size; i++)
            {
                double x = data[i];
                    sum += x * x * x * x * x * x * x +
                           x * x * x * x * x * x +
                           x * x * x * x * x +
                           x * x * x * x +
                           x * x * x +
                           x * x +
                           x;
            }
            return sum;
        }

        // ===================================================================
        // 2. Parallel - Good fan-out, explicit lock for fan-in
        // ===================================================================
        [Benchmark]
        public double Parallel_WithLock()
        {
            double total = 0.0;

            Parallel.For(0L, Size,
                () => 0.0,  // local sum per partition
                (i, state, localSum) =>
                {
                    double x = data[i];
                    localSum += x * x * x * x * x * x * x +
                                x * x * x * x * x * x +
                                x * x * x * x * x +
                                x * x * x * x +
                                x * x * x +
                                x * x +
                                x;
                    return localSum;
                },
                localSum =>
                {
                    lock (lockObject)   // explicit lock instead of Interlocked
                    {
                        total += localSum;
                    }
                });

            return total;
        }
    }
}
