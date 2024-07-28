using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

var directExchange = "direct_Exchange";
var queueName = "ConsumerQueue";

channel.QueueDeclare(queueName, false, false, false, null);

var routingKeys = "letterbox,letterbox1".Split(",", StringSplitOptions.RemoveEmptyEntries);

foreach(var key in routingKeys)
channel.QueueBind(queueName,directExchange,key);

channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);



consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message Received => {message}");
    if (message.Contains("Exception"))
    {
        Console.WriteLine("Error in Processing");
        channel.BasicReject(ea.DeliveryTag, false);
        throw new Exception("Error in Processing");
    }
    if (int.TryParse(message, out var delay))
    {
        Thread.Sleep(delay * 1000);
    }
    channel.BasicAck(ea.DeliveryTag, false);
};

channel.BasicConsume(queueName, false, consumer);

Console.ReadKey();