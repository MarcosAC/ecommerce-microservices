using Xunit;
using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Tests;

public class EstoqueServiceTest
{
    private InventoryDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InventoryDbContext(options);
    }

    [Fact]
    public async Task Deve_Cadastrar_Produto()
    {
        var db = GetDbContext();
        var produto = new Produto { Nome = "Notebook", Descricao = "Dell i7", Preco = 5000, QuantidadeEmEstoque = 10 };

        db.Produtos.Add(produto);
        await db.SaveChangesAsync();

        var encontrado = await db.Produtos.FirstOrDefaultAsync(p => p.Nome == "Notebook");
        Assert.NotNull(encontrado);
        Assert.Equal(10, encontrado.QuantidadeEmEstoque);
    }
}
