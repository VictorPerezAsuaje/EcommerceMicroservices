using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application;
using Services.Orders.Application.PaymentMethods;
using Services.Orders.Domain;

namespace Services.Orders.UI;

[Authorize]
[Route("payment-methods")]
public class PaymentMethodController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(new ResponseDTO<List<PaymentMethodGetDTO>>(true, PaymentMethod.GetAll().ToListGetDTO()));
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, new ResponseDTO(false, "There was an error trying to complete the request"));
        }
    }
}
