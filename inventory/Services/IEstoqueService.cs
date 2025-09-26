using Inventory.Api.Models;
namespace Inventory.Api.Services;
public interface IEstoqueService
{
    Task<Produto> AdicionarProdutoAsync(Produto produto);
    Task<IEnumerable<Produto>> ObterProdutosAsync();
    Task<Produto?> ObterPorIdAsync(int id);
    Task<bool> ReduzirEstoqueAsync(int produtoId, int quantidade);
}
