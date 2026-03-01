using System.Collections.Concurrent;

class Program
{
    private static readonly ConcurrentQueue<byte[]> _boundedQueue = new();
    private static readonly long MaxTotalBytes = 100_000_000; // 100 MB limit
    private static long _currentBytes;

    static void Main()
    {
        Console.WriteLine("Press any key to start...");
        Console.ReadKey();

        int iteration = 0;

        while (true)
        {
            byte[] chunk = new byte[10 * 1024 * 1024];
            for (int i = 0; i < chunk.Length; i += 1000)
                chunk[i] = (byte)(iteration % 256);

            // Manual FIFO eviction
            lock (_boundedQueue) // needed for _currentBytes atomicity
            {
                if (_currentBytes + chunk.Length > MaxTotalBytes)
                {
                    Console.WriteLine($"Queue limit exceeded. Current: {_currentBytes / 1_000_000} MB, trying to add: {chunk.Length / 1_000_000} MB");

                    while (_boundedQueue.TryDequeue(out byte[] old))
                    {
                        _currentBytes -= old.Length;
                    }
                }

                _boundedQueue.Enqueue(chunk);
                _currentBytes += chunk.Length;
            }

            iteration++;
            Console.WriteLine($"Iteration {iteration} — stored {iteration * 10} MB so far (current queue size: {_currentBytes / 1_000_000} MB)");

            if (iteration % 10 == 0)
                Console.ReadKey();
        }
    }
}