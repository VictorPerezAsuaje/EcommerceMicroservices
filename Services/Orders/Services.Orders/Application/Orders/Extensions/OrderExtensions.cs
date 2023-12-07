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
                CurrentStatus = order.CurrentOrderStatus,
                CurrentStatusChangeDate = order.CurrentOrderStatusDate,
                CurrentStatusMessage = order.CurrentOrderStatusMessage ?? "",
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
    {
        OrderStatusHistory lastStatus = order.StatusHistory.OrderByDescending(x => x.ChangeDate).Single();

        return new OrderGetDTO()
        {
            Id = order.Id,
            CurrentStatus = lastStatus.OrderStatus,
            CurrentStatusChangeDate = lastStatus.ChangeDate,
            CurrentStatusMessage = lastStatus.Message,
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
}
