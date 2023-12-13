using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application;
using Services.Orders.Application.Countries;

namespace Services.Orders.UI;

[Authorize]
[Route("countries")]
public class CountryController : ControllerBase
{
    ICountryService _countriesService;

    public CountryController(ICountryService countriesService)
    {
        _countriesService = countriesService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _countriesService.GetAllAsync();
            return Ok(result.ToResponseDTO());
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }
}
