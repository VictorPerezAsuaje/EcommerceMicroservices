using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application;
using Services.Orders.Application.ShippingMethods;

namespace Services.Orders.UI;

[Authorize]
[Route("shipping-methods")]
public class ShippingMethodController : ControllerBase
{
    IShippingMethodService _shippingMethodService;

    public ShippingMethodController(IShippingMethodService shippingMethodService)
    {
        _shippingMethodService = shippingMethodService;
    }

    [HttpGet("{countryName}")]
    public async Task<IActionResult> GetAll(string countryName)
    {
        try
        {
            var result = await _shippingMethodService.GetAllAsync(countryName);
            return Ok(result.ToResponseDTO());
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }
}
