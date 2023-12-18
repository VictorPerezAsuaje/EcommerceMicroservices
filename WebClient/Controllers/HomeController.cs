using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Services.Mailing.Contracts;
using Shared.MessageBus;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
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
    IPublishEndpoint _publisher;

    public HomeController(IAuthService authenticationService, ITokenProvider tokenProvider, ICartService cartService, IPublishEndpoint publisher)
    {
        _authService = authenticationService;
        _tokenProvider = tokenProvider;
        _cartService = cartService;
        _publisher = publisher;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }


    private async Task<IActionResult> ProcessLoginAsync(string token, string? returnUrl = null)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

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

        _tokenProvider.SetToken(token);

        string? fromId = null;
        Request.Cookies.TryGetValue("SessionId", out fromId);

        if (fromId == clientId)
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

            return Redirect(returnUrl ?? "/");
        }

        this.InvokeNotification(x =>
        {
            x.Title = $"Welcome {clientName}!";
            x.Message = "We are glad to see you back 😁";
            x.Icon = NotificationIcon.success;
        });

        return Redirect(returnUrl ?? "/");
    }


    [HttpGet("login")]
    public async Task<IActionResult> LoginAsync()
    {       
        if(User.Identity.IsAuthenticated)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "You are logged in!";
                x.Message = "No worries, no need to log in again 😉";
                x.Icon = NotificationIcon.info;
            });

            return RedirectToAction(nameof(Index));
        }

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

            return await ProcessLoginAsync(result.Value);
        }
        catch (Exception ex)
        {
            // Log
            return RedirectToAction(nameof(Login));
        }
    }

    [HttpGet("oauth/google-auth")]
    public async Task<IActionResult> RequestGoogleAuth(string? returnUrl = null, string? remoteError = null)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            returnUrl = "/";

        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            this.InvokeNotification(x =>
            {
                x.Title = "Login Error";
                x.Message = remoteError;
                x.Icon = NotificationIcon.error;
            });

            return RedirectToAction(nameof(Login));
        }

        try
        {
            var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            var firstName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;
            var lastName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;

            ExternalLoginDataDTO dto = new ExternalLoginDataDTO()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
            };

            var result = await _authService.CompleteGoogleLogin(dto);

            if (result.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Login Error";
                    x.Message = result.Error;
                    x.Icon = NotificationIcon.error;
                });

                return RedirectToAction(nameof(Login));
            }

            return await ProcessLoginAsync(result.Value, returnUrl);
        }
        catch (Exception ex)
        {
            // Log
            return RedirectToAction(nameof(Login));
        }
    }

    [HttpPost("oauth/google-auth")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestGoogleAuth(string provider)
    {
        if (User.Identity.IsAuthenticated)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "You are logged in!";
                x.Message = "No worries, no need to log in again 😉";
                x.Icon = NotificationIcon.info;
            });

            return RedirectToAction(nameof(Index));
        }

        var redirectUrl = Url.Action("RequestGoogleAuth", "Home", new { ReturnUrl = "/" });

        try
        {
            var result = await _authService.GoogleLoginAsync(provider, redirectUrl);

            if (result.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Login Error";
                    x.Message = result.Error;
                    x.Icon = NotificationIcon.error;
                });

                return RedirectToAction(nameof(Login));
            }

            return result.Value;
        }
        catch (Exception ex)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "Login Error";
                x.Message = "There was an error trying to log in using that provider";
                x.Icon = NotificationIcon.error;
            });

            // Log
            return RedirectToAction(nameof(Login));
        }
    }
    

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword()
    {
        if (User.Identity.IsAuthenticated)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "You are logged in!";
                x.Message = "Why would you want to recover your password? 🤔";
                x.Icon = NotificationIcon.info;
            });

            return RedirectToAction(nameof(Index));
        }

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
        if (User.Identity.IsAuthenticated)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "You are logged in!";
                x.Message = "You would need to log out if you intend to register.";
                x.Icon = NotificationIcon.info;
            });

            return RedirectToAction(nameof(Index));
        }

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
            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Logout successful";
                    x.Message = "See you soon!";
                    x.Icon = NotificationIcon.success;
                });

                return RedirectToAction("Index", "Home");
            }

            _tokenProvider.ClearToken();

            Response.Cookies.Delete("SessionId", new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(-1),
                HttpOnly = true,
            });

            this.InvokeNotification(x =>
            {
                x.Title = "Logout successful";
                x.Message = "See you soon!";
                x.Icon = NotificationIcon.success;
            });

            await HttpContext.SignOutAsync();

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
