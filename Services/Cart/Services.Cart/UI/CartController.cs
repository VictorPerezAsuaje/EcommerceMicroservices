using Microsoft.AspNetCore.Mvc;
using Services.Cart.Application;
using Services.Cart.UI.Extensions;

namespace Services.Cart.UI;

[Route("cart")]
public class CartController : Controller
{
    ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetCart(Guid clientId)
    {
        try
        {
            if (clientId == default(Guid))
                return NotFound("The client could not be found.");

            return Ok((await _cartService.GetCartByClientIdAsync(clientId)).ToResponseDTO());
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpPut("{from}/transfer-to/{to}")]
    public async Task<IActionResult> TransferCartItems(Guid from, Guid to)
    {
        if (from == default(Guid))
            return NotFound("The origin could not be found.");

        if (to == default(Guid))
            return NotFound("The destiny could not be found.");

        try
        {
            ResponseDTO result = (await _cartService.TransferCartItemsAsync(from, to)).ToResponseDTO();

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

    [HttpPost("{clientId}")]
    public async Task<IActionResult> AddProductToCart(Guid clientId, [FromBody] CartItemPostDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDTO(false, ModelState.GetErrorsAsString()));

        if (clientId == default(Guid))
            return NotFound("The client could not be found.");

        if(dto.ProductId == default(Guid))
            return NotFound("The product could not be found.");

        try
        {
            ResponseDTO result = (await _cartService.AddCartItemAsync(clientId, dto)).ToResponseDTO();

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

    [HttpPut("{clientId}/{productId}")]
    public async Task<IActionResult> UpdateCartItemAmount(Guid clientId, Guid productId, [FromBody] ItemAmountPutDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDTO(false, ModelState.GetErrorsAsString()));

        if(clientId == default(Guid))
            return NotFound("The client could not be found.");

        if (productId == default(Guid))
            return NotFound("The product could not be found.");

        try
        {
            ResponseDTO result = (await _cartService.UpdateCartItemAmountAsync(clientId, productId, dto)).ToResponseDTO();

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

    [HttpDelete("{clientId}/{productId}")]
    public async Task<IActionResult> RemoveProductFromCart(Guid clientId, Guid productId)
    {
        if (clientId == default(Guid))
            return NotFound(new ResponseDTO(false, "The client could not be found."));

        if (productId == default(Guid))
            return NotFound(new ResponseDTO(false, "The product could not be found."));

        try
        {
            ResponseDTO result = (await _cartService.RemoveCartItemAsync(clientId, productId)).ToResponseDTO();

            if (result.IsFailure)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, new ResponseDTO(false, "There was an error trying to complete the request"));
        }
    }

    [HttpDelete("{clientId}")]
    public async Task<IActionResult> ClearCart(Guid clientId)
    {
        if (clientId == default(Guid))
            return NotFound(new ResponseDTO(false, "The client could not be found."));

        try
        {
            ResponseDTO result = (await _cartService.ClearCartAsync(clientId)).ToResponseDTO();

            if (result.IsFailure)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, new ResponseDTO(false, "There was an error trying to complete the request"));
        }
    }
}
