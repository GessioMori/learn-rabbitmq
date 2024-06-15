using Lib;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool getWarnings = false;
            bool getErrors = false;
            bool getInfos = false;

            List<string> messagesToConsume = new List<string>();

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "-w":
                            getWarnings = true;
                            break;
                        case "-e":
                            getErrors = true;
                            break;
                        case "-i":
                            getInfos = true;
                            break;
                        default:
                            Console.Error.WriteLine($"Unknown argument: '{arg}'");
                            break;
                    }
                }
            }

            if (!(getWarnings || getErrors || getInfos))
            {
                Console.Error.WriteLine("No valid messages to consume.");
                return;
            }
            else
            {
                if (getWarnings)
                {
                    messagesToConsume.Add(MessageType.Warning.ToString());
                }
                if (getErrors)
                {
                    messagesToConsume.Add(MessageType.Error.ToString());
                }
                if (getInfos)
                {
                    messagesToConsume.Add(MessageType.Information.ToString());
                }

                Console.WriteLine($"Consuming: {string.Join(" | ", messagesToConsume)}");
            }

            ConnectionFactory connectionFactory = new() { HostName = "localhost" };

            using IConnection connection = connectionFactory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);

            string queueName = channel.QueueDeclare().QueueName;

            foreach (string type in messagesToConsume)
            {
                channel.QueueBind(queueName, "direct_logs", type);
            }

            Console.WriteLine("Waiting for messages.");

            EventingBasicConsumer consumer = new(channel);

            consumer.Received += (model, ea) =>
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());

                Message? parsedMessage = JsonSerializer.Deserialize<Message>(message);

                if (parsedMessage != null)
                {
                    Console.WriteLine($"[{parsedMessage.Id} - {parsedMessage.MessageType}]: {parsedMessage.Text}");
                }
            };

            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
