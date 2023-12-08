using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders.ViewModels;

public class OrderShippingMethodVM
{
    [Required]
    public string SelectedValue { get; set; }
    public List<ShippingMethodVM> AvailableShippings { get; set; } = new();
}
