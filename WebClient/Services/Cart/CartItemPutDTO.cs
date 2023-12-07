using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Cart;

public class CartItemPutDTO
{
    public string? ThumbnailUrl { get; set; } = null;
    public string? Name { get; set; } = null;

    [Range(0, double.MaxValue)]
    public double? Price { get; set; } = null;

    [Range(0, int.MaxValue)]
    public int? Amount { get; set; } = null;
}
