namespace Sales.Api.Models;
public class Pedido
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string Cliente { get; set; } = string.Empty;
    public List<ItemPedido> Itens { get; set; } = new();
    public string Status { get; set; } = "PENDENTE";
}

public class ItemPedido
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
