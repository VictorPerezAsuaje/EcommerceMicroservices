using Microsoft.AspNetCore.Identity;

namespace Services.Auth.Domain;
public class AppUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
