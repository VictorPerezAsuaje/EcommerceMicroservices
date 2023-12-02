using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Cart;

public class CartItemGetDTO
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }

    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue)]
    public double Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Amount { get; set; }

    [Range(0, 1)]
    public int DiscountApplied { get; set; } = 0;
    public double ComputedPrice => Price * Amount - (Price * Amount * DiscountApplied);
    public bool IsFree => DiscountApplied == 1 || Price == 0;
}
