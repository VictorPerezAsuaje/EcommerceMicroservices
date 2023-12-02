using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services.Catalog.Infrastructure;
using Services.Catalog.Tests.Utilities;

namespace Services.Catalog.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<CatalogDbContext>));

            services.AddSqlServer<CatalogDbContext>("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EcomMicro_Catalog_Tests;Trusted_Connection=true;");
        });

        builder.UseEnvironment("Development");
    }

    public void CleanupDatabase()
    {
        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            dbContext.Database.EnsureDeleted();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
                DbInitializer.SeedData(dbContext);
            }            
        }
    }
}
