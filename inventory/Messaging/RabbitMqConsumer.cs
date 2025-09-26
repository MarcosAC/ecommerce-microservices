using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Inventory.Api.Services;

namespace Inventory.Api.Messaging
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IEstoqueService _estoqueService;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqConsumer(IConfiguration config, IEstoqueService estoqueService)
        {
            _config = config;
            _estoqueService = estoqueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMq:Host"] ?? "localhost",
                UserName = "guest",
                Password = "guest"
            };
            
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: "vendas.queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    var evento = JsonSerializer.Deserialize<VendaEvent>(json);

                    if (evento != null)
                    {
                        foreach (var item in evento.Itens)
                        {
                            await _estoqueService.ReduzirEstoqueAsync(item.ProdutoId, item.Quantidade);
                        }
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMQ] Erro: {ex.Message}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "vendas.queue",
                autoAck: false,
                consumer: consumer
            );
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }

    public record VendaEvent(int PedidoId, List<VendaItem> Itens);
    public record VendaItem(int ProdutoId, int Quantidade);
}
