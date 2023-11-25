namespace Services.Auth.Domain;

public interface IJwtTokenGenerator
{
    string GenerateToken(AppUser user, IEnumerable<string> roles);
}
