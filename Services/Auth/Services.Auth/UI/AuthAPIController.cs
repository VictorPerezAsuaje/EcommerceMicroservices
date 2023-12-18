using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Application;
using Services.Auth.Domain;
using Services.Catalog.UI.Extensions;
using Services.Mailing.Contracts;

namespace Services.Auth.Controllers;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    IAuthService _authService;
    IPublishEndpoint _publisher;

    public AuthController(IAuthService authService, IPublishEndpoint publisher)
    {
        _authService = authService;
        _publisher = publisher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDTO(false, ModelState.GetErrorsAsString()));

        try
        {
            ResponseDTO result = (await _authService.Register(dto)).ToResponseDTO();

            if (result.IsFailure)
                return BadRequest(result);

            _publisher.Publish(new SendEmailRequest()
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.UtcNow,
                To = dto.Email,
                Subject = "Welcome to Ecommerce Microservice!",
                Body = """
                    You have successfully registered your account with us. Please click on the link below to confirm your account:

                    If you were not the one who registered, please feel free to delete your account.
                """
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }        
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDTO(false, ModelState.GetErrorsAsString()));

        try
        {
            var result = (await _authService.Login(dto)).ToResponseDTO();

            if (result.IsFailure)
                return BadRequest(result);

            _publisher.Publish(new SendEmailRequest()
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.UtcNow,
                To = dto.Email,
                Subject = "A login with this email address has been registered",
                Body = """
                    We have noticed a successfull login attempt using this email for the Ecommerce Microservice website.

                    If you were not the one who logged in, we strongly suggest you change your password.
                """
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpPost("login/google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginPostDTO dto)
    {
        try
        {
            var result = _authService.GoogleLogin(dto).ToResponseDTO();

            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }


    [HttpPost("login/complete-google-login")]
    public async Task<IActionResult> CompleteGoogleLogin(ExternalLoginDataDTO dto)
    {
        try
        {
            var result = (await _authService.CompleteGoogleLogin(dto)).ToResponseDTO();

            if (result.IsFailure)
                return BadRequest(result);

            _publisher.Publish(new SendEmailRequest()
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.UtcNow,
                To = dto.Email,
                Subject = "A login with this email address has been registered",
                Body = """
                    We have noticed a successfull login attempt using this email for the Ecommerce Microservice website.

                    If you were not the one who logged in, we strongly suggest you change your password.
                """
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }
}
