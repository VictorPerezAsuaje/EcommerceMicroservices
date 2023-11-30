using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Catalog.Tags;

public class TagPutDTO
{
    [Required]
    public string OldName { get; set; }

    [Required]
    public string NewName { get; set; }
}

