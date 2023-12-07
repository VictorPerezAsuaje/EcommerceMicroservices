using Services.Orders.Infrastructure;


namespace Services.Orders.Tests.Utilities;

internal class DbInitializer
{
    public static void SeedData(OrderDbContext context)
    {
        context.SaveChanges();
    }
}

