namespace WebClient.Services.Cart;

public class CartGetDTO
{
    public Guid Id { get; set; }
    public List<CartItemGetDTO> Items { get; set; } = new ();
    public double SubTotal => Items.Sum(x => x.Price * x.Amount); // No discounts
    public double Total => Items.Sum(x => x.ComputedPrice); // With discounts

    public bool HasDiscounts => Total < SubTotal;
}

