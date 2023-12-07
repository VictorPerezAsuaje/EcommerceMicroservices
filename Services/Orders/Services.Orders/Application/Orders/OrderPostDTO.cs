using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Application.Orders;

public class OrderPostDTO
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemPostDTO> Items { get; set; } = new();

    [Required]
    public string ShippingFirstName { get; set; }

    [Required]
    public string ShippingLastName { get; set; }

    [Required]
    public AddressPostDTO ShippingAddress { get; set; }

    [Required]
    public string ShippingMethod { get; set; }

    [Required]
    public string PaymentMethod { get; set; }

    public bool SaveShippingData { get; set; } = false;
    public string? DiscountCodeApplied { get; set; } = null;
}
