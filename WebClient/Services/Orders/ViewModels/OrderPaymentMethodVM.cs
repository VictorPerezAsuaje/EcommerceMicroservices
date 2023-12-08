using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders.ViewModels;

public class OrderPaymentMethodVM
{
    [Required]
    public string SelectedValue { get; set; }
    public List<PaymentMethodVM> AvailablePayments { get; set; } = new();
}
