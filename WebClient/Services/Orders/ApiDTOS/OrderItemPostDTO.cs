using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Orders.ApiDTOS;

public class OrderItemPostDTO
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The minimum price for an item must be 0.")]
    [DataType(DataType.Currency)]
    public double Price { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The minimum amount for an item must be 1.")]
    public int Amount { get; set; }
}
