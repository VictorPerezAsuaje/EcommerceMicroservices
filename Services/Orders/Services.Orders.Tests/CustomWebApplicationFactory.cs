using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services.Orders.Infrastructure;
using Services.Orders.Tests.Utilities;

namespace Services.Orders.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<OrderDbContext>));

            services.AddDbContext<OrderDbContext>(opts =>
            {
                opts.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EcomMicro_Orders_Tests;Trusted_Connection=true;");
                opts.EnableSensitiveDataLogging();
            });            
        });

        builder.UseEnvironment("Development");
    }

    public void CleanupDatabase()
    {
        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            dbContext.Database.EnsureDeleted();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.ChangeTracker.Clear();
                dbContext.Database.Migrate();
                DbInitializer.SeedData(dbContext);
            }
        }
    }
}
