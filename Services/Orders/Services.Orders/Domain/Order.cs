using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }

    public string ShippingFirstName { get; private set; }
    public string ShippingLastName { get; private set; }
    public Address ShippingAddress { get; private set; }
    public string ShippingMethod { get; private set; }
    public double ShippingFees { get; private set; }

    public string? DiscountCode { get; set; }
    public ClientDiscountCode? DiscountCodeApplied { get; private set; }

    public List<OrderItem> Items { get; private set; } = new();

    public int CurrentOrderStatusId { get; private set; }
    public OrderHistoryItem CurrentOrderStatus { get; private set; }

    public int HistoryId { get; set; }
    public OrderHistory History { get; set; }

    public double SubTotal { get; private set; }
    public double TaxApplied { get; private set; }
    public double Total { get; private set; }

    public DateTime OrderDate { get; private set; }
    public string? ShippingMethodName { get; private set; }
    public ShippingMethod? Shipping { get; private set; }

    public string PaymentMethodName { get; set; }
    public PaymentMethod? PaymentMethod { get; private set; }

    protected Order() { }
    public static Result<Order> Generate(Guid orderId, Guid clientId, string firstName, string lastName, Address shippingAddress, ShippingMethod shippingMethod, List<OrderItem> items, PaymentMethod paymentMethod, double taxApplied = 0.0, DiscountCode? discountCode = null)
    {
        if(clientId == default)
            throw new ArgumentOutOfRangeException("clientId");

        if (shippingMethod.ApplicableFees < 0)
            throw new ArgumentOutOfRangeException("shippingFees");

        if (items.Count == 0)
            return Result.Fail<Order>("The order needs to have at least one item.");

        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Fail<Order>("The first name of the client is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Fail<Order>("The last name of the client is required.");

        if (shippingMethod.CountryName != shippingAddress.CountryName)
            return Result.Fail<Order>("The shipping method selected for the shipping country is not valid.");

        var resultDiscount = discountCode?.UseCode(clientId, orderId);
        if (resultDiscount?.IsFailure ?? false)
            return Result.Fail<Order>(resultDiscount.Error);

        double subtotal = Math.Round(items.Sum(x => x.Price * x.Amount), 2);
        double total = Math.Round(subtotal + subtotal * shippingMethod.ApplicableFees - (subtotal * discountCode?.Value ?? 0.0) - subtotal * taxApplied, 2);


        OrderHistory history = new OrderHistory(orderId);
        OrderHistoryItem firstDraft = history.AddNewStatus(OrderStatus.Draft, "Order draft has been generated.");

        return Result.Ok(new Order()
        {
            Id = orderId,
            ClientId = clientId,
            DiscountCode = discountCode?.Code,
            DiscountCodeApplied = resultDiscount?.Value,
            CurrentOrderStatus = firstDraft,
            History = history,
            ShippingFirstName = firstName,
            ShippingLastName = lastName,
            ShippingAddress = shippingAddress,
            ShippingMethodName = shippingMethod.Name,
            ShippingMethod = shippingMethod.Name,
            ShippingFees = shippingMethod.ApplicableFees,
            Items = items,
            OrderDate = DateTime.UtcNow, 
            PaymentMethodName = paymentMethod.Name,
            SubTotal = subtotal,
            TaxApplied = taxApplied,
            Total = total,
        });     
    }

    #region State changes

    public Order CancelOrder(string? message = null)
    {
        message = message ?? "The cancel request has been applied.";
        CurrentOrderStatus = History.AddNewStatus(OrderStatus.Cancelled, message);         
        return this;
    }


    #endregion State changes
}
