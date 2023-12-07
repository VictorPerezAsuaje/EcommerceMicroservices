using Microsoft.EntityFrameworkCore;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure;
internal class DbInitializer
{
    public static Guid ClientIdOne = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static Guid ClientIdTwo = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static Guid OrderIdOne = Guid.Parse("99999999-1111-1111-1111-111111111111");
    public static Guid OrderIdTwo = Guid.Parse("88888888-2222-2222-2222-222222222222");

    public static Guid ProductIdOne = Guid.Parse("99999999-1111-2222-2222-222222222222");
    public static Guid ProductIdTwo = Guid.Parse("99999999-2222-1111-2222-222222222222");
    public static Guid ProductIdThree = Guid.Parse("99999999-3333-2222-1111-222222222222");
    public static Guid ProductIdFour = Guid.Parse("88888888-2222-2222-2222-222222222222");

    public static List<Country> Countries = new List<Country>()
    {
        new Country("Spain", "ES"),
        new Country("United Kingdom", "UK"),
        new Country("United States of America", "USA"),
        new Country("Canada", "CA"),
        new Country("France", "FR"),
    };

    public static List<ShippingMethod> ShippingMethods = new List<ShippingMethod>()
    {
        new ShippingMethod("FedEx", Countries[0].Name, 0.02),
        new ShippingMethod("MRW", Countries[2].Name, 0.05),
        new ShippingMethod("Nacex", Countries[1].Name, 0.01),
        new ShippingMethod("Correos", Countries[0].Name, 0.02),
    };

    public static List<OrderItem> OrderItems = new List<OrderItem>()
    {
        new OrderItem(OrderIdOne, ProductIdOne, "Product One", 2.95, 2),
        new OrderItem(OrderIdOne, ProductIdTwo, "Product Two", 5.95, 20),
        new OrderItem(OrderIdOne, ProductIdThree, "Product Three", 56.50, 7),
        new OrderItem(OrderIdTwo, ProductIdFour, "Product Four", 56.50, 7)
    };

    public static List<Address> Addresses = new List<Address>()
    {
        Address.Generate(
            country: "Spain",
            countryCode: "ES",
            majorDivision: "Sevilla",
            locality: "Sevilla",
            street: "Test street",
            houseNumber: "52",
            latitude: -39.4566136,
            longitude: 2.33215
        ).Value,
        Address.Generate(
            country: "Spain",
            countryCode: "ES",
            majorDivision: "Sevilla",
            locality: "Sevilla",
            street: "Test street",
            houseNumber: "52",
            latitude: -40.4549936,
            longitude: 5.954656
        ).Value,
    };

    public static List<PaymentMethod> PaymentMethods = new List<PaymentMethod>()
    {
        new PaymentMethod("Paypal"),
        new PaymentMethod("Stripe")
    };

    public static List<Order> Orders = new List<Order>()
    {
        Order.Generate
            (
                orderId: OrderIdOne,
                clientId: ClientIdOne,
                firstName: "Victor",
                lastName: "Pérez Asuaje",
                shippingAddress: Addresses[0],
                shippingMethod: ShippingMethods[0],
                paymentMethod: PaymentMethods[0],
                items: OrderItems.Where(x => x.OrderId == OrderIdOne).ToList(),
                taxApplied: 0.21
            ).Value,
        Order.Generate
            (
                orderId: OrderIdTwo,
                clientId: ClientIdTwo,
                firstName: "Victor",
                lastName: "Pérez Asuaje",
                shippingAddress: Addresses[1],
                shippingMethod: ShippingMethods[3],
                paymentMethod: PaymentMethods[1],
                items: OrderItems.Where(x => x.OrderId == OrderIdTwo).ToList(),
                taxApplied: 0.21
            ).Value,
    };

    public static void SeedData(OrderDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.PaymentMethods.Any())
            context.PaymentMethods.AddRange(PaymentMethods);

        if (!context.ShippingCountries.Any())
            context.ShippingCountries.AddRange(Countries);

        context.SaveChanges();

        if (!context.ShippingMethods.Any())
            context.ShippingMethods.AddRange(ShippingMethods);

        context.SaveChanges();

        if (!context.Orders.Any())
            context.Orders.AddRange(Orders);

        context.SaveChanges();
    }
}


public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<DiscountCode> DiscountCodes { get; set; }
    public DbSet<ClientDiscountCode> ClientDiscountCodes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderHistory> OrderHistory { get; set; }
    public DbSet<OrderHistoryItem> OrderHistoryItems { get; set; }
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
