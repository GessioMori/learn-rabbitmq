using RabbitMQ.Client;
using System.Text;

ConnectionFactory connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
};

using IConnection connection = connectionFactory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "hello-world",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

const string message = "This is the first message.";

byte[] body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(
    exchange: String.Empty,
    routingKey: "hello-world",
    basicProperties: null,
    body: body);

Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");

Console.ReadLine();