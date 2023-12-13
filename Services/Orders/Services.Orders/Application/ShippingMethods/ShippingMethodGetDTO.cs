using Services.Orders.Application.Countries;

namespace Services.Orders.Application.ShippingMethods;

public class ShippingMethodGetDTO
{
    public string Name { get; set; }
    public double ApplicableFees { get; set; }
}
