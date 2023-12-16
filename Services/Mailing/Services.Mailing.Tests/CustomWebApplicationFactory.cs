using Services.Mailing.Infrastructure;
using Services.Mailing.Tests.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Services.Mailing.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<MailingDbContext>));

                services
                    .AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options =>
                    {

                    });

                services.AddAuthorization(opts =>
                {
                    opts.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes("TestScheme")
                        .RequireAuthenticatedUser()
                        .Build();
                });

                services.AddDbContext<MailingDbContext>(opts =>
                {
                    opts.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EcomMicro_Mailing_Tests;Trusted_Connection=true;");
                    opts.EnableSensitiveDataLogging();
                });
            });

            builder.UseEnvironment("Development");
        }

        public void CleanupDatabase()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MailingDbContext>();
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
}
