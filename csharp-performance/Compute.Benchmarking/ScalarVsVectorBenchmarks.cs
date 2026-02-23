using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace Compute.Benchmarking
{
    [ShortRunJob]
    public class ScalarVsVectorBenchmarks
    {
        private const int ArraySize = 10_000_000;
        private readonly float[] data = new float[ArraySize];

        [GlobalSetup]
        public void Setup()
        {
            var rnd = new Random(42);
            for (int i = 0; i < ArraySize; i++)
            {
                data[i] = (float)rnd.NextDouble() * 100f;
            }
        }

        // ────────────────────────────────────────────────
        // 1. Scalar – classic one-element-at-a-time loop
        // ────────────────────────────────────────────────
        [Benchmark(Baseline = true)]
        public float Scalar_FloatMultiplyAndSum()
        {
            float sum = 0f;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i] * 2.0f;
            }
            return sum;
        }

        // ────────────────────────────────────────────────
        // 2. Vector<float> – SIMD on numeric data (good case)
        // ────────────────────────────────────────────────
        [Benchmark]
        public float Vector_FloatMultiplyAndSum()
        {
            float sum = 0f;

            // Vector<float>.Count tells us how many floats fit in a single SIMD register on this CPU. Potentially different from machine to machine...
            // Using this instead of a hard-coded value allows you to take full advantage of wider registers (e.g. AVX-512) when available, while still working on older hardware with narrower registers (e.g. SSE).
            int vectorSize = Vector<float>.Count;

            // "acc" for "accumulator"
            // [0,0,0,0,0,0,0,0]  (for AVX-256)
            Vector<float> acc = Vector<float>.Zero;
            int i;

            for (i = 0; i <= data.Length - vectorSize; i += vectorSize)
            {
                Vector<float> vec = new Vector<float>(data, i);
                acc += vec * 2.0f;
            }

            // reduce the vector accumulator to a single scalar sum
            sum += Vector.Dot(acc, Vector<float>.One);

            // process the remainders that are vectorSize MOD registerSize
            for (; i < data.Length; i++)
            {
                sum += data[i] * 2.0f;
            }

            return sum;
        }
    }
}
