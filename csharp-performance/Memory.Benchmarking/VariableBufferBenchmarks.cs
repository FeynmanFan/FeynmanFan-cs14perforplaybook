using BenchmarkDotNet.Attributes;
using System.Buffers;

namespace Memory.Benchmarking
{
    [MemoryDiagnoser(displayGenColumns: true)]
    [ShortRunJob]
    public class VariableBufferBenchmarks
    {
        private const int Iterations = 1_000_000;
        private static readonly Random _random = new Random(42);

        [Params(true, false)]
        public bool ClearArrayOnReturn { get; set; }

        [Benchmark(Baseline = true)]
        public long NewArray_RandomSizeEachTime()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                int size = _random.Next(64, 8193);           // random size 64–8192 bytes
                byte[] buffer = new byte[size];
                buffer[0] = (byte)i;
                sum += buffer[0];
            }
            return sum;
        }

        [Benchmark]
        public long ReuseSingleFixedArray()
        {
            long sum = 0;
            byte[] buffer = new byte[8192];  // fixed max size – reuse
            for (int i = 0; i < Iterations; i++)
            {
                int size = _random.Next(64, 8193);
                // Simulate using only part of it
                buffer[0] = (byte)i;
                sum += buffer[0];
            }
            return sum;
        }

        [Benchmark]
        public long ArrayPool_RentVariableSize()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                int size = _random.Next(64, 8193);
                byte[] buffer = ArrayPool<byte>.Shared.Rent(size);
                try
                {
                    buffer[0] = (byte)i;
                    sum += buffer[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer, clearArray: ClearArrayOnReturn);
                }
            }
            return sum;
        }
    }
}
