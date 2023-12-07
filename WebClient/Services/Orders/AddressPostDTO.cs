using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders;

public class AddressPostDTO
{
    [Required]
    public string CountryName { get; set; }

    [Required]
    public string MajorDivision { get; set; }
    public string? MajorDivisionCode { get; set; }

    [Required]
    public string Locality { get; set; }
    public string? MinorDivision { get; set; }

    [Required]
    public string Street { get; set; }

    [Required]
    public string StreetNumber { get; set; }

    [Required]
    public string HouseNumber { get; set; }

    [Required]
    public string PostalCode { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
    public string? ExtraDetails { get; set; }
}
