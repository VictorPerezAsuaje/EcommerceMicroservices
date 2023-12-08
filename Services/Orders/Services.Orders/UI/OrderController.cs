using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application;
using Services.Orders.Application.Orders;

namespace Services.Orders.UI;

[Route("orders")]
[Authorize]
public class OrderController : ControllerBase
{
    IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _orderService.GetAllAsync();
            return Ok(result.ToResponseDTO());
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        try
        {
            var result = (await _orderService.GetByIdAsync(id)).ToResponseDTO();

            if (result.IsFailure)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderPostDTO dto)
    {
        if(!ModelState.IsValid)
            return BadRequest(dto);

        try
        {
            ResponseDTO result = (await _orderService.CreateAsync(dto)).ToResponseDTO();

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

    [HttpPut("{id}")]
    public async Task<IActionResult> CancelOrderAsync(Guid id)
    {
        if (id == default)
            return BadRequest("The selected id is not valid");

        try
        {
            ResponseDTO result = (await _orderService.CancelOrderAsync(id)).ToResponseDTO();

            if (result.IsFailure)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }
}
