using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Application.Orders;

public class AddressGetDTO
{
    public string CountryName { get; set; }
    public string MajorDivision { get; set; }
    public string? MajorDivisionCode { get; set; }
    public string Locality { get; set; }
    public string? MinorDivision { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string HouseNumber { get; set; }
    public string PostalCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ExtraDetails { get; set; }
}

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

