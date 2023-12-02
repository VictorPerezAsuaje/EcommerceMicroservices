using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Application.Categories;

public class CategoryPutDTO
{
    [Required]
    public string OldName { get; set; }

    [Required]
    public string NewName { get; set; }
}
