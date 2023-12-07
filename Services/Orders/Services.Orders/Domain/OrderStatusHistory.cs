namespace Services.Orders.Domain;

public class OrderStatusHistory
{
    public int Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }
    public string OrderStatus { get; private set; }
    public DateTime ChangeDate { get; private set; }
    public string Message { get; private set; }

    protected OrderStatusHistory()
    {

    }
    public OrderStatusHistory(Guid orderId, OrderStatus status, string message)
    {
        OrderId = orderId;
        OrderStatus = status.Name;
        ChangeDate = DateTime.UtcNow;
        Message = message;
    }
}
