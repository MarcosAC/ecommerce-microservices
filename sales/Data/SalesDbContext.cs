using Microsoft.EntityFrameworkCore;
using Sales.Api.Models;

namespace Sales.Api.Data;
public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) {}
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }
}
