using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Auth;

public class LoginPostDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public IEnumerable<AuthSchemeDTO> ExternalLogins { get; set; } = new List<AuthSchemeDTO>();

}
