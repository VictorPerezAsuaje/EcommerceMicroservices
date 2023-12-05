using System.ComponentModel.DataAnnotations;
using WebClient.Services.Cart;

namespace WebClient.Services.Orders;

public class OrderCountryPostDTO
{
    [Required]
    public string SelectedValue { get; set; }
    public List<CountryDTO> AvailableCountries { get; set; } = new();
}

public class OrderShippingMethodPostDTO
{
    [Required]
    public string SelectedValue { get; set; }
    public List<ShippingMethodDTO> AvailableShippings { get; set; } = new();
}

public class OrderPaymentMethodPostDTO
{
    [Required]
    public string SelectedValue { get; set; }
    public List<PaymentMethodDTO> AvailablePayments { get; set; } = new();
}

public class OrderPostDTO
{
    [Required]
    public List<OrderItemDTO> Items { get; set; } = new();

    [Required]
    public string ShippingFirstName { get; set; }

    [Required]
    public string ShippingLastName { get; set; }

    [Required]
    public string ShippingAddress { get; set; }

    public OrderCountryPostDTO Country { get; set; }
    public OrderShippingMethodPostDTO Shipping { get; set; }
    public OrderPaymentMethodPostDTO Payment{ get; set; }

    public bool SaveShippingData { get; set; } = false;   

    public double SubTotal => Items.Sum(x => x.Price * x.Amount); // No discounts
    public string? DiscountCodeApplied { get; set; } = null;

    [Range(0, 0.5)]
    public double? DiscountApplied { get; set; } = null;

    [Required]
    public double TaxApplied { get; set; }
    public double Total => Items.Sum(x => x.ComputedPrice); // With discounts
}
