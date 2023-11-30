using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Catalog.Tags;

public class TagPostDTO
{
    [Required]
    public string Name { get; set; }

}
