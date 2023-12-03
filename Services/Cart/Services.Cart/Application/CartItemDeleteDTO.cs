using System.ComponentModel.DataAnnotations;

namespace Services.Cart.Application;

public class CartItemDeleteDTO
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid ProductId { get; set; }
}