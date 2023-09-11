using RabbitMQ.Client;
using System.Text;

ConnectionFactory connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
};

using IConnection connection = connectionFactory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "counter",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine("Send messages by pressing [space]. Exit pressing [enter]");

int numOfMessages = 0;
byte[] body;

while (true)
{
    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

    if (keyInfo.Key == ConsoleKey.Spacebar)
    {
        numOfMessages++;

        body = Encoding.UTF8.GetBytes(numOfMessages.ToString());

        channel.BasicPublish(
            exchange: String.Empty,
            routingKey: "counter",
            basicProperties: null,
            body: body);

        Console.WriteLine($"Number of messages sent: {numOfMessages}");
    }
    else if (keyInfo.Key == ConsoleKey.Enter)
    {
        break;
    }
}