using Services.Orders.Domain;

namespace Services.Orders.Application.Orders;

public static class OrderItemExtensions
{
    public static List<OrderItemGetDTO> ToListGetDTO(this List<OrderItem> items)
        => items.Select(ToGetDTO).ToList();
    public static IQueryable<OrderItemGetDTO> ToQueryableGetDTO(this IQueryable<OrderItem> query)
        => query.Select(item => new OrderItemGetDTO()
        {
            ProductId = item.ProductId,
            Name = item.Name,
            Price = item.Price,
            Amount = item.Amount
        });

    public static OrderItemGetDTO ToGetDTO(this OrderItem item)
        => new OrderItemGetDTO()
        {
            ProductId = item.ProductId,
            Name = item.Name,
            Price = item.Price,
            Amount = item.Amount
        };

    public static List<OrderItem> ToListOrderItem(this List<OrderItemPostDTO> items, Guid orderId)
        => items.Select(x => ToOrderItem(x, orderId)).ToList();
    public static OrderItem ToOrderItem(this OrderItemPostDTO dto, Guid orderId)
        => new OrderItem(
                orderId: orderId,
                productId: dto.ProductId,
                name: dto.Name,
                price: dto.Price,
                amount: dto.Amount
            );
}