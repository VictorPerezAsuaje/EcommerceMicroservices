namespace Services.Orders.Domain;

public class Address
{
    public string CountryName { get; private set; }
    public Country? Country { get; private set; }

    // State, province, oblast, etc.
    public string MajorDivision { get; private set; }
    public string? MajorDivisionCode { get; private set; } = NotApplicable;

    // City, Town, Village, etc.
    public string Locality { get; private set; }

    // City area, district, etc. or N/A
    public string? MinorDivision { get; private set; } = NotApplicable;
    public string Street { get; private set; }

    // Street number or N/A
    public string StreetNumber { get; private set; } = NotApplicable;

    // Street number or N/A
    public string HouseNumber { get; private set; } = NotApplicable;

    // Postal/ZIP Code or N/A
    public string PostalCode { get; private set; } = NotApplicable;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public string? ExtraDetails { get; private set; }

    public const string NotApplicable = "N/A";

    protected Address() { }

    public static Result<Address> Generate(string country, string countryCode, string majorDivision, string locality, string street, double latitude, double longitude,
        string majorDivisionCode = NotApplicable,
        string minorDivision = NotApplicable, 
        string streetNumber = NotApplicable,
        string houseNumber = NotApplicable, 
        string zipCode = NotApplicable,
        string? extraDetails = null
    )
    {
        if (string.IsNullOrWhiteSpace(country))
            return Result.Fail<Address>("The country is required.");        

        if (string.IsNullOrWhiteSpace(majorDivision))
            return Result.Fail<Address>("The major division is required.");

        if (string.IsNullOrWhiteSpace(locality))
            return Result.Fail<Address>("The locality is required.");

        if (string.IsNullOrWhiteSpace(street))
            return Result.Fail<Address>("The street is required.");

        return Result.Ok(new Address()
        {
            CountryName = country,
            MajorDivision = majorDivision,
            MajorDivisionCode = majorDivisionCode,
            Locality = locality,
            MinorDivision = minorDivision,
            Street = street,
            StreetNumber = streetNumber,
            HouseNumber = houseNumber,
            PostalCode = zipCode,
            Latitude = latitude,
            Longitude = longitude,
            ExtraDetails = extraDetails
        });
    } 
}
