namespace Services.Orders.Domain;

public class PaymentMethod
{
    public static PaymentMethod Paypal = new ("Paypal");
    public static PaymentMethod Stripe = new ("Stripe");

    public string Name { get; private set; }

    protected PaymentMethod(string name) {
        Name = name;
    }

    public static List<PaymentMethod> GetAll()
        => new List<PaymentMethod> { Paypal, Stripe };
}