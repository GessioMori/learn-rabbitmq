using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new() { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare("logs", ExchangeType.Fanout);

            string queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName, "logs", string.Empty);

            Console.WriteLine("Waiting for logs. Press [ESC] to exit.");

            EventingBasicConsumer consumer = new(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: " + message);
            };

            while (true)
            {

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Exiting");
                        break;
                    }
                }

                channel.BasicConsume(queueName, true, consumer);

                Thread.Sleep(100);
            }
        }
    }
}
