using RabbitMQ.Client;
using System.Text;

namespace publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new() { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare("logs", ExchangeType.Fanout);

            Console.WriteLine("Sending logs. Press [ESC] to exit.");

            int i = 0;

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

                i++;

                Thread.Sleep(1000);

                string message = CreateLog();
                byte[] body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs", string.Empty, null, body);

                Console.WriteLine($"{i} message(s) sent.");
            }
        }

        static private string CreateLog()
        {
            Guid logId = Guid.NewGuid();
            return $"Message: {logId}";
        }
    }
}
