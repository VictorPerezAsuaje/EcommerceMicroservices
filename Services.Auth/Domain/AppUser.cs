using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Services.Auth.Domain;
public class AppUser : IdentityUser
{
    [Required]
    [ProtectedPersonalData]
    public required string FirstName { get; set; }

    [Required]
    [ProtectedPersonalData]
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    [Required]
    [ProtectedPersonalData]
    public override required string Email { get; set; }

    [Required]
    public override required string UserName { get; set; }
}
