using WebClient.Services.Cart;
using WebClient.Services.Orders.ApiDTOS;

namespace WebClient.Services.Orders.ViewModels;

public static class OrderItemExtensions
{
    public static IEnumerable<OrderItemVM> ToOrderItemList(this IEnumerable<CartItemGetDTO> items)
        => items.Select(ToOrderItem);

    public static OrderItemVM ToOrderItem(this CartItemGetDTO cartItem)
        => new OrderItemVM()
        {
            ProductId = cartItem.ProductId,
            Name = cartItem.Name,
            Price = cartItem.Price,
            Amount = cartItem.Amount
        };


    public static List<OrderItemPostDTO> ToListOrderItemPostDTO(this IEnumerable<OrderItemVM> dto)
        => dto.Select(ToOrderItemPostDTO).ToList();
    public static OrderItemPostDTO ToOrderItemPostDTO(this OrderItemVM dto)
        => new OrderItemPostDTO()
        {
            ProductId = dto.ProductId,
            Name = dto.Name,
            Price = dto.Price,
            Amount = dto.Amount,
        };
}

public static class OrderExtensions
{
    public static AddressPostDTO ToAddressPostDTO(this AddressVM dto)
        => new AddressPostDTO()
        {
            CountryCode = dto.CountryCode,
            CountryName = dto.CountryName,
            MajorDivision = dto.MajorDivision,
            MajorDivisionCode = dto.MajorDivisionCode,
            MinorDivision = dto.MinorDivision,
            Locality = dto.Locality,
            Street  = dto.Street,
            StreetNumber = dto.StreetNumber,
            PostalCode = dto.PostalCode,
            HouseNumber = dto.HouseNumber,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            ExtraDetails = dto.ExtraDetails,
        };

    public static OrderPostDTO ToOrderPostDTO(this OrderVM dto)
        => new OrderPostDTO()
        {
            ClientId = dto.ClientId,
            Items = dto.Items.ToListOrderItemPostDTO(),
            ShippingFirstName = dto.ShippingFirstName,
            ShippingLastName = dto.ShippingLastName,
            ShippingAddress = dto.ShippingAddress.ToAddressPostDTO(),
            ShippingMethod = dto.ShippingMethod,
            PaymentMethod = dto.PaymentMethod,
            DiscountCodeApplied = dto.DiscountCodeApplied,
            SaveShippingData = dto.SaveShippingData,
        };
}