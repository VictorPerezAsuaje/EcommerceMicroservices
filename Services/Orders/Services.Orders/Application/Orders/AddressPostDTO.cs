﻿using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Application.Orders;

public class AddressPostDTO : IValidatableObject
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
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
    public string? ExtraDetails { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Longitude > 90 || Longitude < -90)
            yield return new ValidationResult("Longitude ranges from -90 to 90");

        if (Latitude > 90 || Latitude < -90)
            yield return new ValidationResult("Latitude ranges from -90 to 90");
    }
}

