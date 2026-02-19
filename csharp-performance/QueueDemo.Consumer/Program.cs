using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace QueueDemo.Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var queueName = "fuel.price.us.tx.dfw";

            Console.WriteLine("[Consumer] Starting... Waiting for messages...");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,          // must match publisher settings
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($" [*] Waiting for messages on '{queueName}'. Press ENTER to exit...");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($" [x] Received '{routingKey}' → '{message}'");
                    Console.ResetColor();

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" [x] Error: {ex.Message}");
                    Console.ResetColor();

                    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName,
                                           autoAck: false,
                                           consumer: consumer);

            Console.ReadLine();
            Console.WriteLine("[Consumer] Shutting down...");
        }
    }
}