using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Auth;

public class ForgotPasswordPostDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}
