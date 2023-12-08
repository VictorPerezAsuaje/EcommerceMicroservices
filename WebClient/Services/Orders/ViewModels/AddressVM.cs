using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders.ViewModels;

public class AddressVM : IValidatableObject
{
    [Required]
    public string CountryName => Country?.SelectedValue;

    [Required]
    public string CountryCode { get; set; } = "ES";

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
    public double Latitude => double.TryParse(Pre_Latitude, out double latitude) ? latitude : double.MinValue;
    public string Pre_Latitude { get; set; }

    [Required]
    public double Longitude => double.TryParse(Pre_Longitude, out double longitude) ? longitude : double.MinValue;
    public string Pre_Longitude { get; set; }

    public string? ExtraDetails { get; set; }

    public OrderCountryVM Country { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Longitude > 90 || Longitude < -90)
            yield return new ValidationResult("Longitude ranges from -90 to 90");

        if (Latitude > 90 || Latitude < -90)
            yield return new ValidationResult("Latitude ranges from -90 to 90");
    }

}
