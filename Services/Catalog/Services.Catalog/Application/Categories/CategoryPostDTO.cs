using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Application.Categories;

public class CategoryPostDTO
{
    [Required]
    public string Name { get; set; }
}
