using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Domain;

public class Review
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }    
    public double Rating { get; private set; }
    public string? Title { get; private set; }
    public string? Body { get; private set; }
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }

    protected Review() { /* EF Required */ }
    public Review(Guid productId, string userName, string email, double rating)
    {
        ProductId = productId;
        Username = userName;
        Email = email;
        Rating = rating;
    }

    public Review WithTitle(string title)
    {
        Title = title;
        return this;
    }

    public Review WithBody(string body)
    {
        Body = body; 
        return this;
    }
}