namespace MemoryLeaker
{
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Security.Authentication.ExtendedProtection;

    class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 100 * 1024 * 1024; // 100 MB limit to trigger eviction
                options.CompactionPercentage = .20; // Evict 20% when limit is exceeded
            });

            var provider = services.BuildServiceProvider();
            var cache = provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();

            Console.WriteLine("Press any key to start leaking...");
            Console.ReadKey();

            int iteration = 0;

            while (true)
            {
                // Allocate ~10 MB per iteration and keep reference forever
                byte[] chunk = new byte[10 * 1024 * 1024];
                for (int i = 0; i < chunk.Length; i += 1000)
                    chunk[i] = (byte)(iteration % 256);


                var entryOptions = new MemoryCacheEntryOptions
                {
                    Size = chunk.Length,
                    SlidingExpiration = TimeSpan.FromMinutes(15), // Keep it alive for a while
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Ensure it doesn't expire too soon
                };

                var key = $"leak_{iteration}";
                cache.Set(key, chunk, entryOptions);

                iteration++;
                Console.WriteLine($"Iteration {iteration} — leaked {iteration * 10} MB so far...");

                // Let it run long enough to see memory grow in Task Manager
                if (iteration % 10 == 0)
                    Console.ReadKey(); // pause occasionally so you can observe
            }
        }
    }
}
