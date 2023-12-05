namespace WebClient.Services.Orders;

public class OrderItemDTO
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Amount { get; set; }
    public int DiscountApplied { get; set; } = 0;
    public double ComputedPrice => Price * Amount - (Price * Amount * DiscountApplied);
    public bool IsFree => DiscountApplied == 1 || Price == 0;
}
