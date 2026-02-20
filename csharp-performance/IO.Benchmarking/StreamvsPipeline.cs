using BenchmarkDotNet.Attributes;
using System.IO.Pipelines;

namespace IO.Benchmarking
{
    //[MemoryDiagnoser]
    //[ShortRunJob()]
    //public class LargeFileReadBenchmark
    //{
    //    private const string OUTPUT_PATH = "C:\\code\\cs14pp\\csharp-performance\\IO.Benchmarking\\bin\\Release\\net10.0";

    //    [Params(1, 10)]
    //    public int N;

    //    [Params(8192, 81920)]
    //    public int BufferSizeParam;

    //    [Benchmark(Baseline = true)]
    //    public async Task StreamVersion()
    //    {
    //        await ProcessWithStreamAsync(Path.Combine(OUTPUT_PATH, $"inputfile_{N}.txt"), BufferSizeParam);
    //    }

    //    [Benchmark]
    //    public async Task PipelinesVersion()
    //    {
    //        await ProcessWithPipelinesAsync(Path.Combine(OUTPUT_PATH, $"inputfile_{N}.txt"), BufferSizeParam);
    //    }

    //    private static async Task ProcessWithStreamAsync(string inputPath, int bufferSize)
    //    {
    //        await using var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true);

    //        var buffer = new byte[bufferSize];

    //        while (true)
    //        {
    //            int read = await input.ReadAsync(buffer);
    //            if (read == 0) break;

    //            // Simulate light processing (parse/transform/etc.)
    //            await Task.Delay(1); // fake CPU work per chunk
    //        }
    //    }

    //    private static async Task ProcessWithPipelinesAsync(string inputPath, int bufferSize)
    //    {
    //        await using var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true);

    //        var pipe = new Pipe();

    //        var reading = FillPipeAsync(input, pipe.Writer, bufferSize);
    //        var processing = ProcessPipeAsync(pipe.Reader);

    //        await Task.WhenAll(reading, processing);
    //    }

    //    private static async Task FillPipeAsync(FileStream input, PipeWriter writer, int bufferSize)
    //    {
    //        while (true)
    //        {
    //            var memory = writer.GetMemory(bufferSize);
    //            int bytesRead = await input.ReadAsync(memory);

    //            if (bytesRead == 0) break;

    //            writer.Advance(bytesRead);
    //            var flushResult = await writer.FlushAsync();

    //            if (flushResult.IsCompleted) break;
    //        }

    //        writer.Complete();
    //    }

    //    private static async Task ProcessPipeAsync(PipeReader reader)
    //    {
    //        while (true)
    //        {
    //            var result = await reader.ReadAsync();
    //            var buffer = result.Buffer;

    //            // Simulate processing the chunk
    //            await Task.Delay(1); // fake CPU work

    //            reader.AdvanceTo(buffer.End);

    //            if (result.IsCompleted) break;
    //        }

    //        reader.Complete();
    //    }
    //}
}