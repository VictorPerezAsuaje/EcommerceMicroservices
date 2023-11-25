using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Domain;
using Services.Auth.Infrastructure;

namespace Services.Auth.Application;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;   
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result> Login(LoginRequestDTO request)
    {
        AppUser? user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.Username);

        if (user is null)
            return Result.Fail("Invalid credentials");

        bool isPassValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPassValid)
            return Result.Fail("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return Result.Ok(new UserDTO(user.Id, user.Email, user.FullName));
    }

    public async Task<Result> Register(RegistrationRequestDTO request)
    {
        AppUser user = new()
        {
            UserName = request.Email,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };
 
        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.FirstOrDefault()?.Description ?? "User could not be created.");

        return Result.Ok();
    }
}
