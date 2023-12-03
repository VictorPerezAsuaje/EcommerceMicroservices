using Microsoft.EntityFrameworkCore;
using Services.Cart.Application;
using Services.Cart.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CartDbContext>(opts =>
{
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x =>
        {
            x.MigrationsAssembly(typeof(CartDbContext).Assembly.FullName);
            x.EnableRetryOnFailure(5);
        });
});

builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _context = scope.ServiceProvider.GetService<CartDbContext>();

        if (_context.Database.GetPendingMigrations().Count() > 0)
            _context.Database.Migrate();
    }
}

public partial class Program { }
