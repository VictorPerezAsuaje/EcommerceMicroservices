using Services.Orders.Domain;

namespace Services.Orders.Application.Countries;

public static class CountryExtensions
{
    public static List<CountryGetDTO> ToListGetDTO(this List<Country> countries)
         => countries.Select(ToGetDTO).ToList();
    public static CountryGetDTO ToGetDTO(this Country country)
        => new CountryGetDTO()
        {
            Name = country.Name,
            Code = country.Code,
        };
}