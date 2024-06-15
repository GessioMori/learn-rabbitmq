using Lib;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new() { HostName = "localhost" };

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare("direct_logs", type: ExchangeType.Direct);

            Console.WriteLine("Sending logs. Press [ESC] to exit.");

            int i = 0;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }

                i++;

                Message message = CreateMessage(i);

                channel.BasicPublish(exchange: "direct_logs",
                    routingKey: message.MessageType.ToString(),
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));

                Console.WriteLine($"Message with id {message.Id} and type {message.MessageType} has been sent.");

                Thread.Sleep(1000);
            }
        }

        static Message CreateMessage(int newId)
        {
            Random rand = new Random();
            Array possibleTypes = Enum.GetValues(typeof(MessageType));
            Message message = new Message()
            {
                Id = newId,
                Text = Guid.NewGuid().ToString(),
                MessageType = (MessageType)possibleTypes.GetValue(rand.Next(possibleTypes.Length))!
            };

            return message;
        }
    }


}
