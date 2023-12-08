using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;
using System.Text.Json;
using WebClient.Models;

namespace WebClient.Utilities;

public static class ControllerExtensions
{
    public static Controller InvokeNotification(this Controller controller, Action<Notification> options)
    {
        Notification notification = new Notification();
        options(notification);
        controller.TempData["Notification"] = JsonSerializer.Serialize(notification);
        controller.Response.Headers.Add("HX-Trigger", "notification-added");
        return controller;
    }

}

public static class HtmlHelperValidationExtensions
{
    public static IHtmlContent RedValidationMessage<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        string formName)
    {        
        ArgumentNullException.ThrowIfNull(htmlHelper);
        var htmlAttributes = new { 
            @class = "validation-message text-rose-600", 
            data_valmsg_for = formName
        };
        return htmlHelper.ValidationMessageFor(expression, message: null, htmlAttributes: htmlAttributes, tag: null);
    }
}