using System.ComponentModel.DataAnnotations;

namespace Services.Cart.Application;

public class CartItemPostDTO
{

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string ThumbnailUrl { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public double Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Amount { get; set; }

    [Range(0, 1)]
    public double DiscountApplied { get; set; } = 0;
}
