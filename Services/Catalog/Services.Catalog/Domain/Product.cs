using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Domain;

public class Product
{
    public Guid Id { get; private set; }    
    public string Name { get; private set; }    
    public double Price { get; private set; }    
    public string? Description { get; private set; }
    public string? Details { get; private set; }
    public double Rating => Reviews.Count != 0 ? Reviews.Select(x => x.Rating).Sum() / Reviews.Count : 0;
    public string? CategoryName { get; private set; } = Category.Default;
    public Category? Category { get; private set; }
    public List<Tag> Tags { get; private set; } = new();
    public List<Review> Reviews { get; private set; } = new();

    protected Product() { /* EF Required */ } 

    public Product(Guid id, string name, double price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public Product WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public Product WithDetails(string details)
    {
        Details = details;
        return this;
    }

    public Product WithCategory(string? name)
    {
        CategoryName = name ?? Category.Default;
        return this;
    }

    public Product WithTags(List<Tag> tags)
    {
        Tags = tags;
        return this;
    }

    public Product UpdateBasicData(string name, double price)
    {
        Name = name; 
        Price = price;
        return this;
    }
}
