namespace Services.Orders.Domain;

public class OrderHistory
{
    public int Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }

    public List<OrderHistoryItem> Statuses { get; private set; } = new List<OrderHistoryItem>();

    protected OrderHistory() { }
    public OrderHistory(Guid orderId)
    {
        OrderId = orderId;
    }

    public OrderHistoryItem AddNewStatus(OrderStatus status, string message)
    {
        var newStatus = new OrderHistoryItem(OrderId, status, message);
        Statuses.Add(newStatus);
        return newStatus;
    }
}

public class OrderHistoryItem
{
    public int Id { get; private set; }
    public Guid OrderId { get; private set; }
    public int OrderHistoryId { get; set; }
    public OrderHistory OrderHistory { get; private set; }

    public string OrderStatus { get; private set; }
    public DateTime ChangeDate { get; private set; }
    public string Message { get; private set; }

    protected OrderHistoryItem() { }
    public OrderHistoryItem(Guid orderId, OrderStatus status, string message)
    {
        OrderId = orderId;
        OrderStatus = status.Name;
        ChangeDate = DateTime.UtcNow;
        Message = message;
    }
}

//public class OrderHistoryItem
//{
//    public int Id { get; private set; }
//    public Guid OrderId { get; private set; }
//    public Order Order { get; private set; }

//    public int OrderHistoryId { get; set; }
//    public OrderHistory OrderHistory { get; private set; }

//    public string OrderStatus { get; private set; }
//    public DateTime ChangeDate { get; private set; }
//    public string Message { get; private set; }

//    protected OrderHistoryItem() { }
//    public OrderHistoryItem(Guid orderId, OrderStatus status, string message)
//    {
//        OrderId = orderId;
//        OrderStatus = status.Name;
//        ChangeDate = DateTime.UtcNow;
//        Message = message;
//    }
//}
