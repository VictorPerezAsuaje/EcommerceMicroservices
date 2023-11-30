using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Application.Products;

public class ProductPostDTO
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "The product name is required.")]
    [MaxLength(500)]
    public string Name { get; set; }

    [Required(ErrorMessage = "The price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than {1}.")]
    public double Price { get; set; }
    public string? Category { get; set; } = null;
    public List<string>? Tags { get; set; } = null;

    [MaxLength(2000, ErrorMessage = "The description can only have a maximum of 2000 characters.")]
    public string? Description { get; set; } = null;

    [MaxLength(2000, ErrorMessage = "The description can only have a maximum of 2000 characters.")]
    public string? Details { get; set; } = null;
}
