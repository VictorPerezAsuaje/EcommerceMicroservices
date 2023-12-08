using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders.ViewModels;

public class OrderVM
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemVM> Items { get; set; } = new();

    [Required]
    public string ShippingFirstName { get; set; }

    [Required]
    public string ShippingLastName { get; set; }

    [Required]
    public AddressVM ShippingAddress { get; set; }

    [Required]
    public string ShippingMethod => Shipping?.SelectedValue;

    [Required]
    public string PaymentMethod => Payment?.SelectedValue;

    public OrderShippingMethodVM Shipping { get; set; }
    public OrderPaymentMethodVM Payment { get; set; }

    public bool SaveShippingData { get; set; } = false;

    public string? DiscountCodeApplied { get; set; } = null;
    public double? DiscountApplied { get; set; } = null;
    public double TaxApplied { get; set; }
    public double SubTotal => Items.Sum(x => x.Price * x.Amount);
    public double Total => Items.Sum(x => x.ComputedPrice);
}
