using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Application.Tags;

public class TagPutDTO
{
    [Required]
    public string OldName { get; set; }

    [Required]
    public string NewName { get; set; }
}

