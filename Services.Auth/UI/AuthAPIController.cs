using Microsoft.AspNetCore.Mvc;
using Services.Auth.Application;
using Services.Auth.Domain;
using System.Net;

namespace Services.Auth.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthAPIController : ControllerBase
{
    IAuthService _authService;

    public AuthAPIController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO dto)
    {
        // Simulate bandwidth latency
        await Task.Delay(new Random().Next(50, 200));
        try
        {
            Result result = await _authService.Register(dto);

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
        // Simulate bandwidth latency
        await Task.Delay(new Random().Next(50, 200));

        try
        {
            Result result = await _authService.Login(dto);

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
