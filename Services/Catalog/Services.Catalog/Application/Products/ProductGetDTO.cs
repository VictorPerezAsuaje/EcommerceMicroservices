using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Reviews;
using Services.Catalog.Application.Tags;

namespace Services.Catalog.Application.Products;

public class ProductGetDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public double Rating { get; set; }
    public List<ReviewGetDTO> Reviews { get; set; } = new();
    public CategoryGetDTO? Category { get; set; } = null;
    public List<TagGetDTO> Tags { get; set; } = new();
    public string? Description { get; set; } = null;
    public string? Details { get; set; } = null;
}
