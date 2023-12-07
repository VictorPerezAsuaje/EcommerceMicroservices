using Services.Orders.Domain;

namespace Services.Orders.Application.Orders;

public class OrderStatusHistoryGetDTO
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public string OrderStatus { get; set; }
    public DateTime ChangeDate { get; set; }
    public string Message { get; set; }
}

