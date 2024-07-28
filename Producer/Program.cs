using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

var directExchange = "direct_Exchange";
channel.ExchangeDeclare(directExchange, ExchangeType.Direct, false, false, null);

var routingKey = "letterbox";
var message = "Test";

var encodedMessage = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(directExchange, routingKey, null, encodedMessage);

Console.WriteLine($"Published Message: {message}");