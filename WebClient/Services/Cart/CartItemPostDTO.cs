using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Cart;

public class CartItemPostDTO
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string ThumbnailUrl { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int Amount { get; set; }

    [Range(0, 1)]
    public double DiscountApplied { get; set; } = 0;
}
