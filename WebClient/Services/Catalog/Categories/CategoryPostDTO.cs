using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Catalog.Categories;

public class CategoryPostDTO
{
    [Required]
    public string Name { get; set; }
}
