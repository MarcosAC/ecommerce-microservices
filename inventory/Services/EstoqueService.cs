using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services;
public class EstoqueService : IEstoqueService
{
    private readonly InventoryDbContext _context;
    public EstoqueService(InventoryDbContext context) { _context = context; }

    public async Task<Produto> AdicionarProdutoAsync(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task<IEnumerable<Produto>> ObterProdutosAsync()
        => await _context.Produtos.AsNoTracking().ToListAsync();

    public async Task<Produto?> ObterPorIdAsync(int id)
        => await _context.Produtos.FindAsync(id);

    public async Task<bool> ReduzirEstoqueAsync(int produtoId, int quantidade)
    {
        var prod = await _context.Produtos.FindAsync(produtoId);
        if (prod == null || prod.QuantidadeEmEstoque < quantidade) return false;
        prod.QuantidadeEmEstoque -= quantidade;
        await _context.SaveChangesAsync();
        return true;
    }
}
