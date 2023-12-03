using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebClient.Models;
using WebClient.Services.Auth;

namespace WebClient.Controllers;

[Route("/")]
public class HomeController : Controller
{
    IAuthService _authService;
    ITokenProvider _tokenProvider;
    IWebHostEnvironment _env;

    public HomeController(IAuthService authenticationService, ITokenProvider tokenProvider, IWebHostEnvironment env)
    {
        _authService = authenticationService;
        _tokenProvider = tokenProvider;
        _env = env;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginPostDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var result = await _authService.LoginAsync(dto);

            if(result.IsFailure)
            {
                ModelState.AddModelError("LoginError", result.Error);
                return View(dto);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Value);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaims(new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value),
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value),
                new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value),
                new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value),
            });

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _tokenProvider.SetToken(result.Value);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Log
            return RedirectToAction(nameof(Login));
        }
    }

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost("forgot-password")]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotPasswordPostDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {

        }
        catch (Exception ex)
        {
            // Log
            return RedirectToAction(nameof(ForgotPassword));
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterPostDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var result = await _authService.RegisterAsync(dto);

            if (result.IsFailure)
            {
                ModelState.AddModelError("RegistrationError", result.Error);
                return View(dto);
            }

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            // Log
            return RedirectToAction(nameof(Register));
        }
    }

    [Route("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            await HttpContext.SignOutAsync();

            _tokenProvider.ClearToken();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Log
            return RedirectToAction(nameof(Index));
        }

    }


    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
