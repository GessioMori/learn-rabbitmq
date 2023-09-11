using Lib;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

ConnectionFactory connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
};

using IConnection connection = connectionFactory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "factory",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine("Type worker speed:");

string? choice = Console.ReadLine();

int workerSpeed;

if (int.TryParse(choice, out int num))
{
    workerSpeed = num;
}
else
{
    workerSpeed = 1;
}

EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    string message = Encoding.UTF8.GetString(ea.Body.ToArray());

    FactoryMessage? factoryMessage = JsonSerializer.Deserialize<FactoryMessage>(message);

    if (factoryMessage != null)
    {
        int realDuration = factoryMessage.Duration * 5 / workerSpeed;
        Console.WriteLine($"Started working on message number {factoryMessage.MessageNumber}. It will take {realDuration} milliseconds.");
        Thread.Sleep(realDuration);
        Console.WriteLine($"Finished working on message #{factoryMessage.MessageNumber}.");
    }
    else
    {
        Console.WriteLine("Strange message received!");
    }

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};


channel.BasicConsume(
    queue: "factory",
    autoAck: false,
    consumer: consumer);

Console.ReadLine();