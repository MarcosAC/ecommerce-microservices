using Sales.Api.Data;
using Sales.Api.Models;
using System.Text.Json;
using Inventory.Api.Messaging;
using Sales.Api.Messaging.Events;
using Microsoft.EntityFrameworkCore;

namespace Sales.Api.Services;

public class PedidoService
{
    private readonly SalesDbContext _context;
    private readonly RabbitMqPublisher _publisher;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PedidoService(
        SalesDbContext context,
        RabbitMqPublisher publisher,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _context = context;
        _publisher = publisher;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<Pedido> CriarPedidoAsync(Pedido pedido)
    {
        if (pedido.Itens == null || !pedido.Itens.Any())
            throw new ArgumentException("Pedido precisa ter itens");

        var client = _httpClientFactory.CreateClient("inventory");
        var baseUrl = _configuration["Inventory:BaseUrl"] ?? "http://inventory";
        
        foreach (var item in pedido.Itens)
        {
            var resp = await client.GetAsync($"{baseUrl}/api/produtos/{item.ProdutoId}");
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Produto {item.ProdutoId} não encontrado");

            var json = await resp.Content.ReadAsStringAsync();
            var produto = JsonSerializer.Deserialize<ProdutoDto>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (produto == null || produto.QuantidadeEmEstoque < item.Quantidade)
                throw new InvalidOperationException($"Estoque insuficiente para produto {item.ProdutoId}");
        }
        
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        
        foreach (var item in pedido.Itens)
        {
            var evento = new VendaEvent
            {
                PedidoId = pedido.Id,
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                Data = DateTime.UtcNow
            };

            await _publisher.PublishAsync("estoque_queue", evento);
        }
      
        pedido.Status = "CONFIRMADO";
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();

        return pedido;
    }

    public async Task<Pedido?> ObterPedidoPorIdAsync(int id)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Pedido>> ObterTodosPedidosAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .ToListAsync();
    }

    private record ProdutoDto(int Id, string Nome, string Descricao, decimal Preco, int QuantidadeEmEstoque);
}
