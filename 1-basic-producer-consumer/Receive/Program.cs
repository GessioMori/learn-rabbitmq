using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

Console.WriteLine(" [*] Waiting for messages.");

EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    if (!String.IsNullOrEmpty(message) && int.TryParse(message, out int num))
    {
        Console.WriteLine($" [x] Received {num} time{(num > 1 ? "s" : "")}.");
    }
    else
    {
        Console.WriteLine("Strange message received!");
    }
};

channel.BasicConsume(queue: "counter",
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");

Console.ReadLine();