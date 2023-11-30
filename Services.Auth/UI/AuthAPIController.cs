using Microsoft.AspNetCore.Mvc;
using Services.Auth.Application;
using Services.Auth.Domain;

namespace Services.Auth.Controllers;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO dto)
    {
        try
        {
            ResponseDTO result = (await _authService.Register(dto)).ToResponseDTO();

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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        try
        {
            var result = (await _authService.Login(dto)).ToResponseDTO();

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
}
