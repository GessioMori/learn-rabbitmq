using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
};

using IConnection connection = connectionFactory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "hello-world",
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
    Console.WriteLine($" [x] Received {message}");
};

channel.BasicConsume(queue: "hello-world",
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");

Console.ReadLine();