namespace Services.Orders.Domain;

public class ShippingMethod
{
    public string Name { get; private set; }
    public string CountryName { get; private set; }
    public Country Country { get; private set; }    
    public double ApplicableFees { get; private set; }

    protected ShippingMethod() { }
    public ShippingMethod(string name, string country, double applicableFees)
    {
        if(applicableFees < 0 || applicableFees > 1) throw new ArgumentOutOfRangeException(nameof(applicableFees));
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if(string.IsNullOrWhiteSpace(country)) throw new ArgumentNullException(nameof(country));

        Name = name;
        CountryName = country;
        ApplicableFees = applicableFees;
    }
}