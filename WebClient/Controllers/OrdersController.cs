using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Services.Cart;
using WebClient.Services.Order;
using WebClient.Services.Orders.ViewModels;
using WebClient.Utilities;

namespace WebClient.Controllers;

public static class StaticOrderData
{
    public static List<ShippingMethodVM> ShippingMethods => new()
    {
        new ShippingMethodVM { Name = "FedEx" },
        new ShippingMethodVM { Name = "MRW" },
        new ShippingMethodVM { Name = "Nacex" },
        new ShippingMethodVM { Name = "Correos" }
    };

    public static List<PaymentMethodVM> PaymentMethods => new()
    {
        new PaymentMethodVM { Name = "Paypal" },
        new PaymentMethodVM { Name = "Stripe" }
    };

    public static List<CountryVM> Countries => new() { new() { Name = "Spain" } };
}

[Route("orders")]
public class OrdersController : Controller
{
    ICartService _cartService;
    IOrderService _orderService;

    public OrdersController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [HttpGet("available-countries")]
    public async Task<IActionResult> AvailableCountries(string? selected = null)
    {
        OrderCountryVM dto = new OrderCountryVM()
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
        OrderPaymentMethodVM dto = new OrderPaymentMethodVM()
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
        OrderShippingMethodVM dto = new OrderShippingMethodVM()
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

        OrderVM order = new OrderVM();

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
    public async Task<IActionResult> PlaceOrder(OrderVM order)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Validation error";
                    x.Message = ModelState.GetErrorsAsHtml();
                    x.Icon = NotificationIcon.error;
                });
                
                return View(order);
            }

            string? guid = null;
            Request.Cookies.TryGetValue("SessionId", out guid);

            if (!Guid.TryParse(guid, out Guid clientId))
                return RedirectToAction("Index", "Home");

            order.ClientId = clientId;
            var result = await _orderService.PlaceOrderAsync(clientId, order.ToOrderPostDTO());

            if (result.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Order error";
                    x.Message = result.Error;
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
    }
}
