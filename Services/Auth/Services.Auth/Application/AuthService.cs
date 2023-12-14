using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Domain;
using Services.Auth.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Services.Auth.Application;

public interface IAuthService
{
    Task<Result> Register(RegistrationRequestDTO request);
    Task<Result<string>> Login(LoginRequestDTO request);
    Result<ChallengeResult> GoogleLogin(GoogleLoginPostDTO dto);
    Task<Result<string>> CompleteGoogleLogin(ExternalLoginDataDTO dto);
}

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;   
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    SignInManager<AppUser> _signInManager;

    public AuthService(AuthDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _signInManager = signInManager;
    }

    public async Task<Result<string>> CompleteGoogleLogin(ExternalLoginDataDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        // Create a new user without password if we do not have a user already
        if (user is null)
        {
            user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email,
                Email = dto.Email
            };

            await _userManager.CreateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        // Raise UserLogged event for confirmation email purposes and notification
        return Result.Ok(token);
    }

    public Result<ChallengeResult> GoogleLogin(GoogleLoginPostDTO dto)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(dto.Provider, dto.RedirectUrl);
        return Result.Ok(new ChallengeResult(dto.Provider, properties));
    }

    public async Task<Result<string>> Login(LoginRequestDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Fail<string>("Invalid credentials");

        bool isPassValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPassValid)
            return Result.Fail<string>("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        // Raise UserLogged event for confirmation email purposes and notification

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
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true // Temporary
        };

        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(user, new ValidationContext(user, null, null), validationResults);

        if (!isValid)
            return Result.Fail("There were validation errors trying to create the user.");

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.FirstOrDefault()?.Description ?? "User could not be created.");

        // Raise UserCreated event for confirmation email purposes and notification

        return Result.Ok();
    }
}
