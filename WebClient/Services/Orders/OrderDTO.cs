using System.ComponentModel.DataAnnotations;
using WebClient.Services.Cart;

namespace WebClient.Services.Orders;

public class OrderDTO
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public List<OrderItemDTO> Items { get; set; } = new();

    [Required]
    public string ShippingFirstName { get; set; }

    [Required]
    public string ShippingLastName { get; set; }

    [Required]
    public AddressPostDTO ShippingAddress { get; set; }

    [Required]
    public string ShippingMethod => Shipping.SelectedValue;

    [Required]
    public string PaymentMethod => Payment.SelectedValue;

    public OrderCountryDTO Country { get; set; }
    public OrderShippingMethodDTO Shipping { get; set; }
    public OrderPaymentMethodDTO Payment { get; set; }

    public bool SaveShippingData { get; set; } = false;

    public string? DiscountCodeApplied { get; set; } = null;
    public double? DiscountApplied { get; set; } = null;
    public double TaxApplied { get; set; }
    public double SubTotal => Items.Sum(x => x.Price * x.Amount);    
    public double Total => Items.Sum(x => x.ComputedPrice);
}
