using System.ComponentModel.DataAnnotations;

namespace Services.Auth.Application;

public class RegistrationRequestDTO
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
