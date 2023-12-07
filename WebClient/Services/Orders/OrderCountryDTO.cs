using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders;

public class OrderCountryDTO
{
    [Required]
    public string SelectedValue { get; set; }
    public List<CountryDTO> AvailableCountries { get; set; } = new();
}
