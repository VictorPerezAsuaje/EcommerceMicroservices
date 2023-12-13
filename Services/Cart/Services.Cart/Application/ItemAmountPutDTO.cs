using System.ComponentModel.DataAnnotations;

namespace Services.Cart.Application;

public class ItemAmountPutDTO
{
    [Range(0, int.MaxValue)]
    public int? Amount { get; set; } = null;
}
