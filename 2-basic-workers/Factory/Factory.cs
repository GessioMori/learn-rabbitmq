using Lib;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

ConnectionFactory connectionFactory = new ConnectionFactory
{
    HostName = "localhost"
};

using IConnection connection = connectionFactory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "factory",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine("Production started.");

int minMillis = 500;
int maxMillis = 3000;

int messageNum = 0;

Random rand = new Random();

while (true)
{
    int randomMillis = rand.Next(minMillis, maxMillis);

    Console.WriteLine($"Starting production. It will finish in {randomMillis} milliseconds.");

    Thread.Sleep(randomMillis);

    messageNum++;
    FactoryMessage factoryMessage = new FactoryMessage(messageNum, randomMillis);

    channel.BasicPublish(
        exchange: String.Empty,
        routingKey: "factory",
        basicProperties: null,
        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(factoryMessage)));

    Console.WriteLine($"Message number {factoryMessage.MessageNumber} was sent.");
}

