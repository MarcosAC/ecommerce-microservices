using Microsoft.EntityFrameworkCore;
using Inventory.Api.Models;

namespace Inventory.Api.Data;
public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) {}
    public DbSet<Produto> Produtos { get; set; }
}
