using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Orders.Application.Countries;
using Services.Orders.Application.Orders;
using Services.Orders.Application.ShippingMethods;
using Services.Orders.Infrastructure;
using Services.Orders.UI;
using System.Text;

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
builder.Services.AddScoped<IShippingMethodService, PaymentMethodService>();
builder.Services.AddScoped<ICountryService, CountryService>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

var jwtOptions = new JwtOptions();
builder.Configuration.GetSection("JwtOptions").Bind(jwtOptions);

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true, 
            ValidAudience = jwtOptions.Audience
        };
    });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
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