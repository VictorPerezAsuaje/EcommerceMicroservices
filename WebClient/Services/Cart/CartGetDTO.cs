namespace WebClient.Services.Cart;

public class CartGetDTO
{
    public Guid Id { get; set; }
    public List<CartItemGetDTO> Items { get; set; } = new ();
    public double Total => Items.Sum(x => x.ComputedPrice); 
}

