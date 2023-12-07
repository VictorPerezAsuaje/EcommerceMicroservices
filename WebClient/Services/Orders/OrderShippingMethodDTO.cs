using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders;

public class OrderShippingMethodDTO
{
    [Required]
    public string SelectedValue { get; set; }
    public List<ShippingMethodDTO> AvailableShippings { get; set; } = new();
}
