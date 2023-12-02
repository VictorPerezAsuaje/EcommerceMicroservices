using Microsoft.EntityFrameworkCore;
using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Products;
using Services.Catalog.Application.Tags;
using Services.Catalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(opts =>
{
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x =>
        {
            x.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName);
            x.EnableRetryOnFailure(5);
        });
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

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
        var _context = scope.ServiceProvider.GetService<CatalogDbContext>();

        if (_context.Database.GetPendingMigrations().Count() > 0)
            _context.Database.Migrate();
    }
}

public partial class Program { }