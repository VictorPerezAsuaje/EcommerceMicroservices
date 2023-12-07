using Services.Orders.Domain;
using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Application.Orders;

public class OrderGetDTO
{
    public Guid Id { get; set; }
    public string CurrentStatus { get; set; }
    public DateTime CurrentStatusChangeDate { get; set; }
    public string CurrentStatusMessage { get; set; }
    public string ShippingFirstName { get; set; }
    public string ShippingLastName { get; set; }

    public AddressGetDTO ShippingAddress { get; set; }

    public string ShippingMethod { get; set; }
    public double ShippingFees { get; set; }
    public string PaymentMethod { get; set; }

    public List<OrderItemGetDTO> Items { get; set; } = new();

    public double SubTotal => Items.Sum(x => x.ComputedPrice);
    public string? DiscountCodeApplied { get; set; } = null;
    public double DiscountApplied { get; set; } = 0.0;
    public double TaxApplied { get; set; }
    public double Total => SubTotal + SubTotal * TaxApplied - (SubTotal * DiscountApplied); 
}

