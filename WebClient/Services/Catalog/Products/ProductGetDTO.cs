using WebClient.Services.Catalog.Reviews;
using WebClient.Services.Catalog.Categories;
using WebClient.Services.Catalog.Tags;

namespace WebClient.Services.Catalog.Products;

public class ProductGetDTO
{
    /* CatalogService */

    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public double Rating { get; set; }
    public List<ReviewGetDTO> Reviews { get; set; } = new();
    public CategoryGetDTO? Category { get; set; } = null;
    public List<TagGetDTO> Tags { get; set; } = new();
    public string? Description { get; set; } = null;
    public string? Details { get; set; } = null;

    /* InventoryService */

    public int AvailableInStock { get; set; } = 5;
    public bool InStock => AvailableInStock > 0;

}
