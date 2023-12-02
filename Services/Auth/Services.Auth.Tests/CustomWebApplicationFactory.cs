using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services.Auth.Application;
using Services.Auth.Infrastructure;
using Services.Auth.Tests.Utilities;
using System.Data.Common;

namespace Services.Auth.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AuthDbContext>));

            services.AddSqlServer<AuthDbContext>("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EcomMicro_Auth_Tests;Trusted_Connection=true;");
        });

        builder.UseEnvironment("Development");
    }

    public void CleanupDatabase()
    {
        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            dbContext.Database.EnsureDeleted();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
                DbInitializer.SeedData(dbContext);
            }
        }
    }
}
