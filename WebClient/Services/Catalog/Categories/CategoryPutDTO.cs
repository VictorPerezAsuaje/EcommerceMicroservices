using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Catalog.Categories;

public class CategoryPutDTO
{
    [Required]
    public string OldName { get; set; }

    [Required]
    public string NewName { get; set; }
}
