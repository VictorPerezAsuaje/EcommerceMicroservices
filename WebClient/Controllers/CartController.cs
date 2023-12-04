using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebClient.Models;
using WebClient.Services.Cart;
using WebClient.Utilities;

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

            this.InvokeNotification(x =>
            {
                x.Title = "Error loading cart";
                x.Message = "There was an error trying to load the cart.";
                x.Icon = NotificationIcon.error;
            });
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
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error adding the product";
                    x.Message = "The product could not be added.";
                    x.Icon = NotificationIcon.error;
                });

                return BadRequest();
            }

            var result = await _cartService.AddCartItemAsync(clientId, dto);

            if (result.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error adding the product";
                    x.Message = "The product could not be added.";
                    x.Icon = NotificationIcon.error;
                });

                return BadRequest();
            }

            Response.Headers.Add("HX-Trigger-After-Swap", "cart-item-added");
        }
        catch (Exception ex)
        {
            // Log
            this.InvokeNotification(x =>
            {
                x.Title = "Error adding the product";
                x.Message = "There was an error trying to add the product to the cart.";
                x.Icon = NotificationIcon.error;
            });
        }

        return Ok();
    }

    [HttpDelete("{productId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        try
        {
            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error removing item";
                    x.Message = "The product could not be removed.";
                    x.Icon = NotificationIcon.error;
                });

                return BadRequest();
            }
                

            var result = await _cartService.RemoveCartItemAsync(clientId, productId);

            if (result.IsFailure)
                return BadRequest();

            Response.Headers.Add("HX-Trigger-After-Swap", "cart-item-added");
        }
        catch (Exception ex)
        {
            // Log
            this.InvokeNotification(x =>
            {
                x.Title = "Error removing the product";
                x.Message = "There was an error trying to remove the product from the cart.";
                x.Icon = NotificationIcon.error;
            });
        }

        return Ok();
    }

    [HttpPut("{productId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCartItemAmount(Guid productId, [FromBody]int amount)
    {
        try
        {
            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error updating item";
                    x.Message = "The product could not be updated.";
                    x.Icon = NotificationIcon.error;
                });

                return BadRequest();
            }

            CartItemPutDTO dto = new CartItemPutDTO() { Amount = amount };
            var result = await _cartService.UpdateCartItemAsync(clientId, productId, dto);

            if (result.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error updating item";
                    x.Message = "The product could not be updated.";
                    x.Icon = NotificationIcon.error;
                });

                return BadRequest();
            }

            Response.Headers.Add("HX-Trigger-After-Swap", "cart-item-added");
        }
        catch (Exception ex)
        {
            // Log
            this.InvokeNotification(x =>
            {
                x.Title = "Error updating the product";
                x.Message = "There was an error trying to update the product.";
                x.Icon = NotificationIcon.error;
            });
        }

        return Ok();
    }
}
