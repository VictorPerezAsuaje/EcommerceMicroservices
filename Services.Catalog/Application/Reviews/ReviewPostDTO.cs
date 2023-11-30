using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Application.Reviews;

public class ReviewPostDTO
{
    [Required(ErrorMessage = "The username is required.")]
    [MaxLength(500)]
    public string Username { get; set; }

    [Required(ErrorMessage = "The email is required.")]
    [DataType(DataType.EmailAddress, ErrorMessage = "The email provided is not valid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The email is required.")]
    [Range(0, 5, ErrorMessage = "The rating must be between 0 and 5.")]
    public double Rating { get; set; }

    [MaxLength(500)]
    public string? Title { get; set; } = null;

    [MaxLength(2000)]
    public string? Body { get; set; } = null;
}
