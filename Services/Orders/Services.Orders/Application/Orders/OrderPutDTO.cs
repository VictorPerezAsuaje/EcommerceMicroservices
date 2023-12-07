using System.ComponentModel.DataAnnotations;

namespace Services.Orders.Application.Orders;

public class OrderPutDTO
{
    [Required]
    public string OldName { get; set; }

    [Required]
    public string NewName { get; set; }
}
