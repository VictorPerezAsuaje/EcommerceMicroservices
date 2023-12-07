namespace Services.Orders.Domain;

public class OrderItem
{
    public Guid OrderId { get; private set; }
    public Order? Order { get; set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public int Amount { get; private set; }
    public double ComputedPrice => Price * Amount;
    public bool IsFree => Price == 0;

    protected OrderItem() { /* EF */}
    public OrderItem(Guid orderId, Guid productId, string name, double price, int amount)
    {
        OrderId = orderId;  
        ProductId = productId;
        Name = name;
        Price = price;
        Amount = amount;
    }
}