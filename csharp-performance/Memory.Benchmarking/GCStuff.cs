using BenchmarkDotNet.Attributes;
using System.Buffers;

namespace Memory.Benchmarking
{
    [MemoryDiagnoser(displayGenColumns: true)]
    [ShortRunJob()]
    public class GcPressureBenchmarks
    {
        private const int Iterations = 1_000_000;
        private static readonly Random _random = new Random(42);

        [Benchmark(Baseline = true)]
        public long GCWorstCase_NewAllocationEveryIteration()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                byte[] buffer = new byte[64];           // high Gen 0 pressure – new allocation every time  
                buffer[0] = (byte)i;
                sum += buffer[0];
            }
            return sum;
        }

        [Benchmark]
        public long GCBadCase_TinyObjectCreation()
        {
            long sum = 0;
            var dummyList = new List<object>(Iterations);  // ← force allocation

            for (int i = 0; i < Iterations; i++)
            {
                var obj = new object();
                dummyList.Add(obj);           // ← JIT can't eliminate — reference escapes
                sum += i;
            }
            return sum;
        }

        [Benchmark]
        public long GCBetterCase_ArrayPoolReuse()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(64);
                try
                {
                    buffer[0] = (byte)i;
                    sum += buffer[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);  // low pressure – reuse pool
                }
            }
            return sum;
        }

        [Benchmark]
        public long GCBestCase_ReuseSingleArray()
        {
            long sum = 0;
            byte[] buffer = new byte[64];               // allocate once outside loop
            for (int i = 0; i < Iterations; i++)
            {
                buffer[0] = (byte)i;
                sum += buffer[0];
            }
            return sum;
        }

        [Benchmark]
        public long GCZeroPressure_StackallocAndSpan()
        {
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                Span<byte> buffer = stackalloc byte[64];  // zero heap allocation – stack only - STACK OVERFLOW, but no GC pressure
                buffer[0] = (byte)i;
                sum += buffer[0];
            }
            return sum;
        }
    }
}