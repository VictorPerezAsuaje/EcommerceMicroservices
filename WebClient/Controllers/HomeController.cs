using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebClient.Models;
using WebClient.Services.Auth;
using WebClient.Services.Cart;
using WebClient.Utilities;

namespace WebClient.Controllers;

[Route("/")]
public class HomeController : Controller
{
    IAuthService _authService;
    ICartService _cartService;
    ITokenProvider _tokenProvider;
    IWebHostEnvironment _env;

    public HomeController(IAuthService authenticationService, ITokenProvider tokenProvider, IWebHostEnvironment env, ICartService cartService)
    {
        _authService = authenticationService;
        _tokenProvider = tokenProvider;
        _env = env;
        _cartService = cartService;
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

            var clientId = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            var clientEmail = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value;
            var clientName = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value;

            identity.AddClaims(new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, clientEmail),
                new Claim(JwtRegisteredClaimNames.Sub, clientId),
                new Claim(JwtRegisteredClaimNames.Name, clientName),
                new Claim(ClaimTypes.Name, clientName),
            });

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _tokenProvider.SetToken(result.Value);

            string? fromId = null;
            Request.Cookies.TryGetValue("SessionId", out fromId);

            if(fromId == clientId)
                return RedirectToAction(nameof(Index));

            Response.Cookies.Delete("SessionId");
            Response.Cookies.Append("SessionId", clientId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMonths(1),
                HttpOnly = true,
            });

            if (!Guid.TryParse(fromId, out Guid fromGuid))
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error transfering items";
                    x.Message = "There was an error trying to transfer the items you had in your cart before authentication.";
                    x.Icon = NotificationIcon.error;
                });

                return RedirectToAction(nameof(Index));
            }

            var response = await _cartService.TransferCartItemsAsync(fromGuid, Guid.Parse(clientId));

            if (response.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error transfering items";
                    x.Message = response.Error;
                    x.Icon = NotificationIcon.error;
                });
            }

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

    [HttpGet("Notify")]
    public async Task<IActionResult> Notify()
    {
        return PartialView("Partials/_Notification");
    }


    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
