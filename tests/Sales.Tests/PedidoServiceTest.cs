using Xunit;
using Sales.Api.Data;
using Sales.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Sales.Tests;

public class PedidoServiceTest
{
    private SalesDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<SalesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SalesDbContext(options);
    }

    [Fact]
    public async Task Deve_Criar_Pedido()
    {
        var db = GetDbContext();
        var pedido = new Pedido
        {
            Cliente = "João",
            Status = "PENDENTE",
            Itens = new List<ItemPedido>
            {
                new ItemPedido { ProdutoId = 1, Quantidade = 2 }
            }
        };

        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync();

        var encontrado = await db.Pedidos.FirstOrDefaultAsync(p => p.Cliente == "João");
        Assert.NotNull(encontrado);
        Assert.Equal("PENDENTE", encontrado.Status);
    }
}
