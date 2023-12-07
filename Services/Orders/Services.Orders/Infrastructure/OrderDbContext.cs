using Microsoft.EntityFrameworkCore;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<DiscountCode> DiscountCodes { get; set; }
    public DbSet<ClientDiscountCode> ClientDiscountCodes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderStatusHistory> OrderHistories { get; set; }
    public DbSet<OrderStatus> OrderStatus { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<ClientShippingData> ClientShippingData { get; set; }
    public DbSet<ShippingMethod> ShippingMethods { get; set; }
    public DbSet<Country> ShippingCountries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
