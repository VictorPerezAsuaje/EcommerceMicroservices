namespace Services.Cart.Domain;

public class CartItem
{
    public Guid ClientId { get; private set; }
    public Guid ProductId { get; private set; }

    public string ThumbnailUrl { get; private set; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public int Amount { get; private set; }
    public double DiscountApplied { get; private set; } = 0;
    public double ComputedPrice => Price * Amount - (Price * Amount * DiscountApplied);
    public bool IsFree => DiscountApplied == 1 || Price == 0;

    public CartItem(Guid clientId, Guid productId, string thumbnailUrl, string name, double price, int amount)
    {
        ClientId = clientId;
        ProductId = productId;
        Name = name;
        ThumbnailUrl = thumbnailUrl;

        if (price < 0) price = 0;

        Price = price;

        if (amount < 0) amount = 0;

        Amount = amount;        
    }

    public CartItem ApplyDiscount(double discount)
    {
        if (discount < 0) discount = 0;
        if (discount > 1) discount = 1;

        DiscountApplied = discount;

        return this;
    }
}
