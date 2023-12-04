using Services.Cart.Domain;
using Services.Cart.Infrastructure;

namespace Services.Cart.Tests;

internal class DbInitializer
{
    public static Guid ClientIdOne = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static Guid ClientIdTwo = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static List<CartItem> CartItems = new()
    {
        new CartItem(ClientIdOne, Guid.Parse("12111111-1111-1111-1111-111111111111"), "thumbnail1.jpg", "Product A", 19.99, 2),
        new CartItem(ClientIdOne, Guid.Parse("13111111-1111-1111-1111-111111111111"), "thumbnail2.jpg", "Product B", 29.95, 1),
        new CartItem(ClientIdOne, Guid.Parse("14111111-1111-1111-1111-111111111111"), "thumbnail3.jpg", "Product C", 39.99, 3),
        new CartItem(ClientIdOne, Guid.Parse("15111111-1111-1111-1111-111111111111"), "thumbnail4.jpg", "Product D", 15.00, 4),
        new CartItem(ClientIdOne, Guid.Parse("16111111-1111-1111-1111-111111111111"), "thumbnail5.jpg", "Product E", 25.50, 2),
        new CartItem(ClientIdOne, Guid.Parse("17111111-1111-1111-1111-111111111111"), "thumbnail6.jpg", "Product F", 49.99, 1),
        new CartItem(ClientIdOne, Guid.Parse("18111111-1111-1111-1111-111111111111"), "thumbnail7.jpg", "Product G", 9.99, 5),
        new CartItem(ClientIdOne, Guid.Parse("19111111-1111-1111-1111-111111111111"), "thumbnail8.jpg", "Product H", 34.50, 2),
        new CartItem(ClientIdTwo, Guid.Parse("12111111-1111-1111-1111-111111111111"), "thumbnail1.jpg", "Product A", 19.99, 10),
        new CartItem(ClientIdTwo, Guid.Parse("22111111-1111-1111-1111-111111111111"), "thumbnail9.jpg", "Product I", 29.99, 3),
        new CartItem(ClientIdTwo, Guid.Parse("23111111-1111-1111-1111-111111111111"), "thumbnail10.jpg", "Product J", 44.75, 1),
    };

    public static void SeedData(CartDbContext context)
    {
        if (!context.CartItems.Any())
            context.CartItems.AddRange(CartItems);

        context.SaveChanges();
    }
}

