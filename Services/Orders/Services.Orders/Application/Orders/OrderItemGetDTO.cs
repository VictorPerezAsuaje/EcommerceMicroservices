using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Application.Orders;

public class OrderItemGetDTO
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }

    [DataType(DataType.Currency)]
    public double Price { get; set; }
    public int Amount { get; set; }

    [DataType(DataType.Currency)]
    public double ComputedPrice => Price * Amount;
    public bool IsFree => Price == 0;
}

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