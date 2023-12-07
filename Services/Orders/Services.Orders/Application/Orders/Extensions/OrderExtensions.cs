using Services.Orders.Domain;

namespace Services.Orders.Application.Orders;

public static class OrderExtensions
{
    public static List<OrderGetDTO> ToListGetDTO(this List<Order> orders)
        => orders.Select(ToGetDTO).ToList();
    public static IQueryable<OrderGetDTO> ToQueryableGetDTO(this IQueryable<Order> query)
        => query.Select(order => new OrderGetDTO()
            {
                Id = order.Id,
                CurrentStatus = order.CurrentOrderStatus.OrderStatus,
                CurrentStatusChangeDate = order.CurrentOrderStatus.ChangeDate,
                CurrentStatusMessage = order.CurrentOrderStatus.Message,
                ShippingFirstName = order.ShippingFirstName,
                ShippingLastName = order.ShippingLastName,
                ShippingAddress = order.ShippingAddress.ToGetDTO(),
                ShippingMethod = order.ShippingMethod,
                ShippingFees = order.ShippingFees,
                PaymentMethod = order.PaymentMethodName,
                Items = order.Items.ToListGetDTO(),
                TaxApplied = order.TaxApplied,
                DiscountCodeApplied = order.DiscountCode,
                DiscountApplied = order.DiscountCodeApplied != null ? order.DiscountCodeApplied.Value : 0.0
            }
        );

    public static OrderGetDTO ToGetDTO(this Order order)    
        => new OrderGetDTO()
        {
            Id = order.Id,
            CurrentStatus = order.CurrentOrderStatus.OrderStatus,
            CurrentStatusChangeDate = order.CurrentOrderStatus.ChangeDate,
            CurrentStatusMessage = order.CurrentOrderStatus.Message,
            ShippingFirstName = order.ShippingFirstName,
            ShippingLastName = order.ShippingLastName,
            ShippingAddress = order.ShippingAddress.ToGetDTO(),
            ShippingMethod = order.ShippingMethod,
            ShippingFees = order.ShippingFees,
            PaymentMethod = order.PaymentMethodName,
            Items = order.Items.ToListGetDTO(),
            TaxApplied = order.TaxApplied,
            DiscountCodeApplied = order.DiscountCode,
            DiscountApplied = order.DiscountCodeApplied?.Value ?? 0.0
        };
    
}
