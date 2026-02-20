using BenchmarkDotNet.Attributes;
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

        // ===================================================================
        // 1. Classic FileStream (Random Access)
        // ===================================================================
        [Benchmark(Baseline = true)]
        public long Stream_RandomResourceLoad()
        {
            long sum = 0;
            using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, true);

            for (int i = 0; i < NumResourceLoads; i++)
            {
                long offset = (long)(_random.NextDouble() * (FileSizeBytes - 64));
                fs.Seek(offset, SeekOrigin.Begin);

                Span<byte> buffer = stackalloc byte[64];
                fs.Read(buffer);
                sum += MemoryMarshal.Read<long>(buffer); // simulate using the data
            }
            return sum;
        }

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