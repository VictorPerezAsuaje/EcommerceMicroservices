﻿namespace Services.Cart.Domain;

public class CartItem
{
    // Whether SessionId or Authenticated ClientId
    public Guid ClientId { get; private set; } 
    public Guid ProductId { get; private set; }

    public string ThumbnailUrl { get; private set; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public int Amount { get; private set; }
    public double ComputedPrice => Price * Amount;
    public bool IsFree => Price == 0;
    public DateTime? ExpirationTime { get; private set; }

    protected CartItem() { /* EF */}

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

    public CartItem RenewExpirationTime()
    {
        ExpirationTime = ExpirationTime is null ? DateTime.UtcNow : DateTime.UtcNow.AddMonths(1);
        return this;
    }

    public CartItem IncreaseAmount(int amountToAdd)
    {
        if (amountToAdd < 0) return this;

        Amount += amountToAdd;
        return this;
    }

    public CartItem UpdateAmount(int newAmount)
    {
        Amount = newAmount;
        return this;
    }
}
