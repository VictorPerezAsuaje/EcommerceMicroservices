

namespace Services.Cart.Application;

public class CartItemGetDTO
{
    public Guid ProductId { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Amount { get; set; }
    public double DiscountApplied { get; set; }
    public double ComputedPrice { get; set; }
    public bool IsFree { get; set; }
}
