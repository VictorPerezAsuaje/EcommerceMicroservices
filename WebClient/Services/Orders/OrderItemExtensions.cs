using WebClient.Services.Cart;

namespace WebClient.Services.Orders;

public static class OrderItemExtensions
{
    public static IEnumerable<OrderItemDTO> ToOrderItemList(this IEnumerable<CartItemGetDTO> items)
        => items.Select(ToOrderItem);

    public static OrderItemDTO ToOrderItem(this CartItemGetDTO cartItem)
        => new OrderItemDTO()
        {
            ProductId = cartItem.ProductId,
            Name = cartItem.Name,
            Price = cartItem.Price,
            Amount = cartItem.Amount,
            DiscountApplied = cartItem.DiscountApplied,
            
        };
}
