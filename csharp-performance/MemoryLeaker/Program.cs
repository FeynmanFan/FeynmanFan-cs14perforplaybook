namespace MemoryLeaker
{
    using System;
    using System.Collections.Generic;

    class Program
    {
        // This static list keeps growing forever → classic leak
        private static readonly List<byte[]> _leakCache = new List<byte[]>();

        static void Main()
        {
            Console.WriteLine("Press any key to start leaking...");
            Console.ReadKey();

            int iteration = 0;

            while (true)
            {
                // Allocate ~10 MB per iteration and keep reference forever
                byte[] chunk = new byte[10 * 1024 * 1024];
                for (int i = 0; i < chunk.Length; i += 1000)
                    chunk[i] = (byte)(iteration % 256);

                _leakCache.Add(chunk);

                iteration++;
                Console.WriteLine($"Iteration {iteration} — leaked {iteration * 10} MB so far...");

                // Let it run long enough to see memory grow in Task Manager
                if (iteration % 10 == 0)
                    Console.ReadKey(); // pause occasionally so you can observe
            }
        }
    }
}
