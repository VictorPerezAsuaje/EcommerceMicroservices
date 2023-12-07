using Services.Orders.Domain;

namespace Services.Orders.Application.Orders;

public static class AddressExtensions
{
    public static Result<Address> ToAddress(this AddressPostDTO dto)
        => Address.Generate(
                country: dto.CountryName,
                countryCode: dto.CountryCode,
                majorDivision: dto.MajorDivision,
                locality: dto.Locality,
                street: dto.Street,
                latitude: dto.Latitude,
                longitude: dto.Longitude,
                majorDivisionCode: dto.MajorDivisionCode,
                minorDivision: dto.MinorDivision,
                streetNumber: dto.StreetNumber,
                houseNumber: dto.HouseNumber,
                zipCode: dto.PostalCode,
                extraDetails: dto.ExtraDetails
            );

    public static AddressGetDTO ToGetDTO(this Address address)
        => new AddressGetDTO()
        {
            CountryName = address.CountryName,
            MajorDivision = address.MajorDivision,
            MajorDivisionCode = address.MajorDivisionCode,
            Locality = address.Locality,
            MinorDivision = address.MinorDivision,
            Street = address.Street,
            StreetNumber = address.StreetNumber,
            HouseNumber = address.HouseNumber,
            PostalCode = address.PostalCode,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            ExtraDetails = address.ExtraDetails
        };
}