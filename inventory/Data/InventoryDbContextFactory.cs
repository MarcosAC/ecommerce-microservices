using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Inventory.Api.Data;

namespace Inventory.Api.Data
{
    public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
    {
        public InventoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();

            optionsBuilder.UseSqlServer("ConnectionStrings__Default=Server=sqlserver;Database=SalesDb;User Id=sa;Password=Your_password123");

            return new InventoryDbContext(optionsBuilder.Options);
        }
    }
}
