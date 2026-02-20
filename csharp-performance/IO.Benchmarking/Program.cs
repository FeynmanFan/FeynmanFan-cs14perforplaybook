using BenchmarkDotNet.Running;
using performance_helper;

namespace IO.Benchmarking
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //await DataHelper.CreateLargeGameAssetFileAsync("game_assets_5MB.dat", 5_000_000L);   // 5M
            //await DataHelper.CreateLargeGameAssetFileAsync("game_assets_8GB.dat", 8_000_000_000L);   // 8 GB

            //DataHelper.CreateLargeTestFile("inputfile_1.txt", 1);
            //DataHelper.CreateLargeTestFile("inputfile_10.txt", 10);

            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
