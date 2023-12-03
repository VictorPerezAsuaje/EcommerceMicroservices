using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Cart;

public class CartItemDeleteDTO
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid ProductId { get; set; }
}