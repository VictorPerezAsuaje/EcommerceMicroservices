namespace Services.Orders.Domain;

public class ClientShippingData
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public string ShippingFirstName { get; private set; }
    public string ShippingLastName { get; private set; }
    public Address ShippingAddress { get; private set; }

    protected ClientShippingData()
    {
        
    }

    public ClientShippingData(Guid clientId, string firstName, string lastName, Address address)
    {
        ClientId = clientId;
        ShippingFirstName = firstName;
        ShippingLastName = lastName;
        ShippingAddress = address;
    }
}
