namespace Services.Orders.Domain;

public class ShippingMethod
{
    public string Name { get; private set; }
    public double ApplicableFees { get; private set; }
    public List<Country> Countries { get; private set; }

    protected ShippingMethod() { }
    public ShippingMethod(string name, double applicableFees)
    {
        if(applicableFees < 0 || applicableFees > 1) throw new ArgumentOutOfRangeException(nameof(applicableFees));
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        Name = name;
        ApplicableFees = applicableFees;
    }

    public ShippingMethod WithValidCountries(List<Country> countries)
    {
        if(countries == null) return this;

        Countries = countries;
        return this;
    }
}