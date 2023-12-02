using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Application.Tags;

public class TagPostDTO
{
    [Required]
    public string Name { get; set; }

}
