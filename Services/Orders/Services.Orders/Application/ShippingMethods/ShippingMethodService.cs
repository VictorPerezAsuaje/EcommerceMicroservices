using Microsoft.EntityFrameworkCore;
using Services.Orders.Domain;
using Services.Orders.Infrastructure;

namespace Services.Orders.Application.ShippingMethods;

public interface IShippingMethodService
{
    Task<Result<List<ShippingMethodGetDTO>>> GetAllAsync(string countryName);
}

public class PaymentMethodService : IShippingMethodService
{
    private readonly OrderDbContext _context;

    public PaymentMethodService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ShippingMethodGetDTO>>> GetAllAsync(string countryName)
        => Result.Ok(
            await _context.ShippingMethods
            .Include(x => x.Countries)
            .Where(x => x.Countries.Any(x => x.Name == countryName))
            .Select(x => x.ToGetDTO())
            .AsNoTracking()
            .ToListAsync()
        );
}
