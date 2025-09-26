using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Inventory.Api.Messaging
{
    public class RabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher(string hostName, string userName, string password, string virtualHost = "/")
        {
            _factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                VirtualHost = virtualHost
            };
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            await using var connection = await _factory.CreateConnectionAsync();

            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
          
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };
            
            await channel.BasicPublishAsync<BasicProperties>(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
        }
    }
}
