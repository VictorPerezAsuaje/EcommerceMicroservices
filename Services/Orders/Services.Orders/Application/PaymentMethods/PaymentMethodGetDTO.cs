using Services.Orders.Application.Countries;

namespace Services.Orders.Application.PaymentMethods;

public class PaymentMethodGetDTO
{
    public string Name { get; set; }
    public double ApplicableFees { get; set; }
}
