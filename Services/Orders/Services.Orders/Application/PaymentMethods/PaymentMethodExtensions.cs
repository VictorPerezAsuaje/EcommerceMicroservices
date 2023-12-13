using Services.Orders.Domain;

namespace Services.Orders.Application.PaymentMethods;

public static class PaymentMethodExtensions
{
    public static List<PaymentMethodGetDTO> ToListGetDTO(this IEnumerable<PaymentMethod> payments)
         => payments.Select(ToGetDTO).ToList();
    public static PaymentMethodGetDTO ToGetDTO(this PaymentMethod payment)
        => new PaymentMethodGetDTO()
        {
            Name = payment.Name
        };
}