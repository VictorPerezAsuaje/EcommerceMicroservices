namespace WebClient.Services.Orders.ViewModels;

public class OrderItemVM
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Amount { get; set; }
    public double ComputedPrice => Price * Amount;
    public bool IsFree => Price == 0;
}
