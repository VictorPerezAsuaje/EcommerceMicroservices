using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Services.Cart;
using WebClient.Services.Orders;
using WebClient.Utilities;

namespace WebClient.Controllers;

public static class StaticOrderData
{
    public static List<ShippingMethodDTO> ShippingMethods => new()
    {
        new ShippingMethodDTO { Name = "FedEx" },
        new ShippingMethodDTO { Name = "MRW" },
        new ShippingMethodDTO { Name = "Nacex" },
        new ShippingMethodDTO { Name = "Correos" }
    };

    public static List<PaymentMethodDTO> PaymentMethods => new()
    {
        new PaymentMethodDTO { Name = "Paypal" },
        new PaymentMethodDTO { Name = "Stripe" }
    };

    public static List<CountryDTO> Countries => new() { new() { Name = "Spain" } };
}

[Route("orders")]
public class OrdersController : Controller
{
    ICartService _cartService;

    public OrdersController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("available-countries")]
    public async Task<IActionResult> AvailableCountries(string? selected = null)
    {
        OrderCountryPostDTO dto = new OrderCountryPostDTO()
        {
            SelectedValue = selected,
            AvailableCountries = StaticOrderData.Countries
        };

        Response.Headers.Add("HX-Trigger-After-Swap", "reload-validators");
        return PartialView("Partials/_OrderCountries", dto);
    }

    [HttpGet("payment-methods")]
    public async Task<IActionResult> PaymentMethods(string? selected = null)
    {
        OrderPaymentMethodPostDTO dto = new OrderPaymentMethodPostDTO()
        {
            SelectedValue = selected,
            AvailablePayments = StaticOrderData.PaymentMethods
        };

        Response.Headers.Add("HX-Trigger-After-Swap", "reload-validators");
        return PartialView("Partials/_PaymentMethods", dto);
    }

    [HttpGet("shipping-methods")]
    public async Task<IActionResult> ShippingMethods(string? selected = null)
    {
        OrderShippingMethodPostDTO dto = new OrderShippingMethodPostDTO()
        {
            SelectedValue = selected,
            AvailableShippings = StaticOrderData.ShippingMethods
        };

        Response.Headers.Add("HX-Trigger-After-Swap", "reload-validators");

        return PartialView("Partials/_ShippingMethods", dto);
    }

    [HttpGet("PlaceOrder")]
    public async Task<IActionResult> PlaceOrder()
    {
        if (!User.Identity.IsAuthenticated)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "Login required";
                x.Message = "You need to log in before you can complete your order.";
                x.Icon = NotificationIcon.error;
            });

            return RedirectToAction("Login", "Home");
        }

        OrderPostDTO order = new OrderPostDTO();

        try
        {
            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
                return RedirectToAction("Index", "Home");

            var result = await _cartService.GetCartByClientIdAsync(clientId);

            if (result.IsFailure)
                return RedirectToAction("Index", "Home");

            order.Items = result.Value.ToOrderItemList().ToList();
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

        return View(order);
    }

    [HttpPost("PlaceOrder")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(OrderPostDTO order)
    {
        try
        {
            throw new NotImplementedException();

            if (!ModelState.IsValid)
                return View(order);
        }
        catch (Exception ex)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "Error placing the order cart";
                x.Message = "There was an error trying to place the order.";
                x.Icon = NotificationIcon.error;
            });

            return View(order);
        }

        this.InvokeNotification(x =>
        {
            x.Title = "Order completed successfully";
            x.Icon = NotificationIcon.success;
        });
        return RedirectToAction("Index", "Home");
    }
}
