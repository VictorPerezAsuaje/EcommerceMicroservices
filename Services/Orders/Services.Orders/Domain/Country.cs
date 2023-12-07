namespace Services.Orders.Domain;

public class Country
{
    public string Name { get; private set; }
    public string Code { get; set; }
    public Country(string name, string code)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name"); 
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException("code");
        
        Name = name;
        Code = code;
    }
}
