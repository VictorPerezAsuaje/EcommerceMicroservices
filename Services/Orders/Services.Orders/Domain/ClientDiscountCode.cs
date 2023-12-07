namespace Services.Orders.Domain;

public class ClientDiscountCode
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public DiscountCode? DiscountCode { get; private set; }
    public double Value { get; set; }
    public Guid ClientId { get; private set; }
    public DateTime UsageDate { get; private set; }
    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }

    /* Generation throught the DiscountCode.UseCode() method */
    public ClientDiscountCode(string code, double value, Guid clientId, Guid orderId)
    {
        if(string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException("code");
        if(clientId == default) throw new ArgumentNullException("clientId");
        if(orderId == default) throw new ArgumentNullException("orderId");

        Code = code;
        Value = value;
        ClientId = clientId;
        OrderId = orderId;
        UsageDate = DateTime.UtcNow;
    }
}
