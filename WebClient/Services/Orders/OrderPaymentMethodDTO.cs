using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders;

public class OrderPaymentMethodDTO
{
    [Required]
    public string SelectedValue { get; set; }
    public List<PaymentMethodDTO> AvailablePayments { get; set; } = new();
}
