using Services.Orders.Domain;

namespace Services.Orders.Application.ShippingMethods;

public static class ShippingMethodExtensions
{
    public static List<ShippingMethodGetDTO> ToListGetDTO(this IEnumerable<ShippingMethod> shippings)
         => shippings.Select(ToGetDTO).ToList();
    public static ShippingMethodGetDTO ToGetDTO(this ShippingMethod shipping)
        => new ShippingMethodGetDTO()
        {
            Name = shipping.Name,
            ApplicableFees = shipping.ApplicableFees,
        };
}