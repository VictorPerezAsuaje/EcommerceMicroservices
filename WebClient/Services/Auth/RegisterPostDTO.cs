using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Auth;

public class RegisterPostDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public string? PhoneNumber { get; set; }
}
