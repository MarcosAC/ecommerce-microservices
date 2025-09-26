using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sales.Api.Data
{
    public class SalesDbContextFactory : IDesignTimeDbContextFactory<SalesDbContext>
    {
        public SalesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SalesDbContext>();

            optionsBuilder.UseSqlServer("ConnectionStrings__Default=Server=sqlserver;Database=SalesDb;User Id=sa;Password=Your_password123");

            return new SalesDbContext(optionsBuilder.Options);
        }
    }
}
