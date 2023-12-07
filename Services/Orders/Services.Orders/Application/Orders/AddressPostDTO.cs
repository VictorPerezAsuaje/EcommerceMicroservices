using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Application.Orders;

public class AddressPostDTO
{
    [Required]
    public string CountryName { get; set; }

    [Required]
    public string CountryCode { get; set; }

    [Required]
    public string MajorDivision { get; set; }
    public string? MajorDivisionCode { get; set; }

    [Required]
    public string Locality { get; set; }
    public string? MinorDivision { get; set; }

    [Required]
    public string Street { get; set; }
    public string? StreetNumber { get; set; }
    public string? HouseNumber { get; set; }
    public string? PostalCode { get; set; }

    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude ranges from -90 to 90")]
    public double Latitude { get; set; }

    [Required]
    [Range(-90, 90, ErrorMessage = "Longitude ranges from -90 to 90")]
    public double Longitude { get; set; }
    public string? ExtraDetails { get; set; }
}

