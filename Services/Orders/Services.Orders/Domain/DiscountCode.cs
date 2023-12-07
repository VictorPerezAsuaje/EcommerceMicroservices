namespace Services.Orders.Domain;

public class DiscountCode
{
    public string Code { get; private set; }
    public double Value { get; private set; }

    public int NumberOfUses { get; private set; }   
    public int MaxUsage { get; private set; } = 1;
    public int MaxUsagePerClient { get; private set; } = 1;
    public bool MaxUsageReached => NumberOfUses >= MaxUsage;
    public DateTime ValidUntil { get; private set; }
    public bool IsExpired => DateTime.UtcNow > ValidUntil;
    public Guid? AssociatedClientId { get; private set; }
    public bool IsUserSpecific => AssociatedClientId is not null;

    protected DiscountCode()
    {

    }
    public DiscountCode(string code, double value, DateTime validUntil, int maxUsage = 1, int usagePerClient = 1)
    {
        if (value > 1 || value < 0)  throw new ArgumentOutOfRangeException("value");
        if (maxUsage < 0)  throw new ArgumentOutOfRangeException("maxUsage");
        if (usagePerClient < 0)  throw new ArgumentOutOfRangeException("usagePerClient");
        if (validUntil == default)  throw new ArgumentOutOfRangeException("validUntil");

        Code = code;
        Value = value;
        MaxUsage = maxUsage;
        MaxUsagePerClient = usagePerClient;
        ValidUntil = validUntil;
    }

    public DiscountCode MadeFor(Guid clientId)
    {
        AssociatedClientId = clientId;
        return this;
    }

    public Result<ClientDiscountCode> UseCode(Guid clientId, Guid orderId)
    {
        if (IsExpired)
            return Result.Fail<ClientDiscountCode>("The code is expired.");

        if (IsUserSpecific && clientId != AssociatedClientId)
            return Result.Fail<ClientDiscountCode>("This code can not be used by this user.");

        if (MaxUsageReached)
            return Result.Fail<ClientDiscountCode>("The code has reached the maximum usage.");

        NumberOfUses += 1;
        return Result.Ok(new ClientDiscountCode(Code, Value, clientId, orderId));
    }
}
