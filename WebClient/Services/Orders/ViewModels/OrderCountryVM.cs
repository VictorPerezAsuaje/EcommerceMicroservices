using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders.ViewModels;

public class OrderCountryVM
{
    [Required]
    public string SelectedValue { get; set; }
    public List<CountryVM> AvailableCountries { get; set; } = new();
}
