using RabbitMQ.Client;
using System.Text;

namespace QueueDemo.Publisher
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Publisher started. Sending messages directly to queue...");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            var queues = new [] 
            { 
                "fuel.price.us.tx.dfw",
                "fuel.price.ca.yyz",
                "fuel.price.eu.lhr",
                "weather.alert.tx.houston",
                "fuel.price.us.fl.mia",
                "fuel.price.us.tx.iah"
            };   

            // Declare a simple durable queue (messages survive broker restart if durable=true)
            foreach(var queue in queues)
            {
                 await channel.QueueDeclareAsync(
                    queue: queue,
                    durable: true,          // survives broker restart
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }

            // Messages (no topics/routing keys needed anymore)
            var messages = new[]
            {
                new { Queue = "fuel.price.us.tx.dfw",     Content = "Jet-A $5.89/gal at KDFW" },
                new { Queue = "fuel.price.ca.yyz",     Content = "Jet-A $6.12/gal at YYZ" },
                new { Queue = "fuel.price.eu.lhr",        Content = "Jet-A £4.95/litre at EGLL" },
                new { Queue = "weather.alert.tx.houston", Content = "Thunderstorm warning IAH" },
                new { Queue = "fuel.price.us.fl.mia",     Content = "Jet-A $5.67/gal at KMIA" },
                new { Queue = "fuel.price.us.tx.iah",     Content = "Jet-A $5.95/gal at KIAH" },
                new { Queue = "fuel.price.us.tx.dfw",     Content = "Jet-A $5.39/gal at KDFW" },
            };

            foreach (var msg in messages)
            {
                var body = Encoding.UTF8.GetBytes(msg.Content);

                // Publish directly to the queue (exchange = "")
                await channel.BasicPublishAsync(
                    exchange: string.Empty,          // ← empty = direct to queue
                    routingKey: msg.Queue,           // ← queue name goes here
                    mandatory: false,
                    basicProperties: new BasicProperties(),
                    body: body,
                    cancellationToken: CancellationToken.None);

                Console.WriteLine($" [x] Sent to '{msg.Queue}': '{msg.Content}'");
                await Task.Delay(800);
            }

            Console.WriteLine("All messages sent. Press any key to exit...");
            Console.ReadKey();
        }
    }
}