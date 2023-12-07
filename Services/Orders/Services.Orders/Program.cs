using Microsoft.EntityFrameworkCore;
using Services.Orders.Application.Orders;
using Services.Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(opts =>
{
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x =>
        {
            x.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName);
            x.EnableRetryOnFailure(5);
        });
});

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
        var _context = scope.ServiceProvider.GetService<OrderDbContext>();

        if (_context.Database.GetPendingMigrations().Count() > 0)
            _context.Database.Migrate();

        if (app.Environment.IsDevelopment())
        {
            DbInitializer.SeedData(_context);
        }
    }
}

public partial class Program { }