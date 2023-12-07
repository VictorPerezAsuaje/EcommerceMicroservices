namespace Services.Orders.Domain;

public class OrderStatus
{ 
    /* Semaphore */
    public static OrderStatus Draft = new("Draft", "Draft");
    public static OrderStatus PendingPayment = new("PendingPayment", "Pending Payment");
    public static OrderStatus Paid = new("Paid", "Paid");
    public static OrderStatus PendingShipping = new("PendingShipping", "Pending Shipping");
    public static OrderStatus Shipped = new("Shipped", "Shipped");
    public static OrderStatus PendingDelivery = new("PendingDelivery", "Pending Delivery");
    public static OrderStatus Delivered = new("Delivered", "Delivered");
    public static OrderStatus Completed = new("Completed", "Completed");

    /* User Actions */
    public static OrderStatus Cancelled = new("Cancelled", "Cancelled");

    /* Error states */
    public static OrderStatus InformationRequired = new("InformationRequired", "Information Required");
    public static OrderStatus PaymentError = new("PaymentError", "Payment Error");
    public static OrderStatus ShippingError = new("ShippingError", "Shipping Error");
    public static OrderStatus DeliveryError = new("DeliveryError", "Delivery Error");

    public string Alias { get; private set; }
    public string Name { get; private set; }

    protected OrderStatus(string alias, string name) { 
        Alias = alias;
        Name = name;
    }
};
