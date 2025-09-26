namespace Sales.Api.Messaging.Events;

public class VendaEvent
{
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public DateTime Data { get; set; }
}