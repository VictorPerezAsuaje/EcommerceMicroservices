using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Domain;
using Services.Auth.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Services.Auth.Application;

public interface IAuthService
{
    Task<Result> Register(RegistrationRequestDTO request);
    Task<Result<string>> Login(LoginRequestDTO request);
}

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;   
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(AuthDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<string>> Login(LoginRequestDTO request)
    {
        AppUser? user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.Username);

        if (user is null)
            return Result.Fail<string>("Invalid credentials");

        bool isPassValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPassValid)
            return Result.Fail<string>("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return Result.Ok(token);
    }

    public async Task<Result> Register(RegistrationRequestDTO request)
    {
        bool exists = _context.Users.Any(x => x.Email == request.Email);

        if (exists)
            return Result.Fail<string>("The email used to create the account already exists");

        AppUser user = new()
        {
            UserName = request.Email,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };

        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(user, new ValidationContext(user, null, null), validationResults);

        if (!isValid)
            return Result.Fail("There were validation errors trying to create the user.");

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.FirstOrDefault()?.Description ?? "User could not be created.");

        return Result.Ok();
    }
}
