using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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