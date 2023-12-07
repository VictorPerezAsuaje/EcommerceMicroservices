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

    public double ComputedPrice => Price * Amount;
    public bool IsFree => Price == 0;
}
