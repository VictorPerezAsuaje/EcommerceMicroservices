using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebClient.Services.Cart;

namespace WebClient.Controllers;

[Route("cart")]
public class CartController : Controller
{
    ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Cart()
    {
        CartGetDTO cart = new CartGetDTO();

        try
        {
            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
                return PartialView("Partials/_Cart", cart);

            var result = await _cartService.GetCartByClientIdAsync(clientId);

            if (result.IsFailure)
                return PartialView("Partials/_Cart", cart);

            cart.Items = result.Value;
        }
        catch (Exception ex)
        {
            // Log
        }

        return PartialView("Partials/_Cart", cart);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(CartItemPostDTO dto)
    {
        try
        {
            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
                return BadRequest();

            var result = await _cartService.AddCartItemAsync(clientId, dto);

            if (result.IsFailure)
                return BadRequest();

            Response.Headers.Add("HX-Trigger-After-Swap", "cart-item-added");
        }
        catch (Exception ex)
        {
            // Log
        }

        return Ok();
    }

    [HttpDelete("{productId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        try
        {
            // Set cartData in temporary table based on user's SessionId
            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
                return BadRequest();

            var result = await _cartService.RemoveCartItemAsync(clientId, productId);

            if (result.IsFailure)
                return BadRequest();

            Response.Headers.Add("HX-Trigger-After-Swap", "cart-item-added");
        }
        catch (Exception ex)
        {
            // Log
        }

        return Ok();
    }

    [HttpPut("{productId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCartItemAmount(Guid productId, [FromBody]int amount)
    {
        try
        {
            // Set cartData in temporary table based on user's SessionId
            if (!User.Identity.IsAuthenticated)
            {
                return Ok();
            }

            string? guid = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(guid, out Guid clientId))
                return BadRequest();

            CartItemPutDTO dto = new CartItemPutDTO() { Amount = amount };
            var result = await _cartService.UpdateCartItemAsync(clientId, productId, dto);

            if (result.IsFailure)
                return BadRequest();

            Response.Headers.Add("HX-Trigger-After-Swap", "cart-item-added");
        }
        catch (Exception ex)
        {
            // Log
        }

        return Ok();
    }
}
