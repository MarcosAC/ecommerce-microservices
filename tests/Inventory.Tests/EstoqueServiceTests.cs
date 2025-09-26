using Xunit;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Data;
using Inventory.Api.Services;
using Inventory.Api.Models;

public class EstoqueServiceTests
{
    [Fact]
    public async Task ReduzirEstoque_DeveriaDiminuirQuantidade()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase("TestDb1")
            .Options;

        using var ctx = new InventoryDbContext(options);
        ctx.Produtos.Add(new Produto { Nome = "T", QuantidadeEmEstoque = 5, Preco = 10 });
        await ctx.SaveChangesAsync();

        var service = new EstoqueService(ctx);
        var ok = await service.ReduzirEstoqueAsync(1, 2);
        Assert.True(ok);
        var p = await service.ObterPorIdAsync(1);
        Assert.Equal(3, p!.QuantidadeEmEstoque);
    }
}
