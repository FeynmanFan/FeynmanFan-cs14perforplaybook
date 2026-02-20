using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace GameAssetLoadingBenchmarks
{
    [MemoryDiagnoser]
    [ShortRunJob()]
    public class GameAssetLoadingBenchmark
    {
        private const string OUTPUT_PATH = "C:\\code\\cs14pp\\csharp-performance\\IO.Benchmarking\\bin\\Release\\net10.0";

        private const string File5MB = "game_assets_5MB.dat";
        private const string File8GB = "game_assets_8GB.dat";

        [Params(5_000_000L, 8_000_000_000L)]
        public long FileSizeBytes { get; set; }

        private string _filePath = string.Empty;
        private readonly Random _random = new Random(42);

        private const int NumResourceLoads = 100_000;   // Simulate loading 100k resources

        [GlobalSetup]
        public void GlobalSetup()
        {
            _filePath = FileSizeBytes switch
            {
                5_000_000L => File5MB,
                8_000_000_000L => File8GB,
                _ => throw new ArgumentException()
            };

            _filePath = Path.Combine(OUTPUT_PATH, _filePath);
        }

        /// <summary>
        /// Stack-safe version of the stream code, FYI. 
        /// Actual run results on my machine:
        //  | Method                          | FileSizeBytes | Mean         | Error         | StdDev     | Ratio | RatioSD | Gen0      | Allocated  | Alloc Ratio |
        //|-------------------------------- |-------------- |-------------:|--------------:|-----------:|------:|--------:|----------:|-----------:|------------:|
        //| Stream_RandomResourceLoad       | 5000000       |  3,616.84 ms |  1,963.086 ms | 107.603 ms | 1.001 |    0.04 | 8000.0000 | 17582048 B |       1.000 |
        //| MemoryMapped_RandomResourceLoad | 5000000       |     11.55 ms |      6.937 ms |   0.380 ms | 0.003 |    0.00 |         - |      298 B |       0.000 |
        //|                                 |               |              |               |            |       |         |           |            |             |
        //| Stream_RandomResourceLoad       | 8000000000    | 20,316.51 ms | 14,138.582 ms | 774.983 ms |  1.00 |    0.05 | 8000.0000 | 17610736 B |       1.000 |
        //| MemoryMapped_RandomResourceLoad | 8000000000    |    554.47 ms |    468.372 ms |  25.673 ms |  0.03 |    0.00 |         - |     1088 B |       0.000 |
        /// </summary>
        [Benchmark(Baseline = true)]
        public long Stream_RandomResourceLoad()
        {
            long sum = 0;

            using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, true);

            byte[] buffer = ArrayPool<byte>.Shared.Rent(64);
            try
            {
                Span<byte> bufSpan = buffer.AsSpan(0, 64);

                for (int i = 0; i < NumResourceLoads; i++)
                {
                    long offset = (long)(_random.NextDouble() * (FileSizeBytes - 64));
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.ReadExactly(bufSpan);
                    sum += MemoryMarshal.Read<long>(bufSpan);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return sum;
        }

        //// ===================================================================
        //// 1. Classic FileStream (Random Access)
        //// ===================================================================
        //[Benchmark(Baseline = true)]
        //public long Stream_RandomResourceLoad()
        //{
        //    long sum = 0;
        //    using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, true);

        //    for (int i = 0; i < NumResourceLoads; i++)
        //    {
        //        long offset = (long)(_random.NextDouble() * (FileSizeBytes - 64));
        //        fs.Seek(offset, SeekOrigin.Begin);

        //        Span<byte> buffer = stackalloc byte[64];
        //        fs.Read(buffer);
        //        sum += MemoryMarshal.Read<long>(buffer); // simulate using the data
        //    }
        //    return sum;
        //}

        // ===================================================================
        // 2. Memory-Mapped File (Random Access)
        // ===================================================================
        [Benchmark]
        public long MemoryMapped_RandomResourceLoad()
        {
            long sum = 0;

            using var mmf = MemoryMappedFile.CreateFromFile(
                _filePath,
                FileMode.Open,
                null,
                0,
                MemoryMappedFileAccess.Read);

            using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

            for (int i = 0; i < NumResourceLoads; i++)
            {
                long offset = (long)(_random.NextDouble() * (FileSizeBytes - 64));
                sum += accessor.ReadInt64(offset);   // direct memory read
            }
            return sum;
        }
    }
}