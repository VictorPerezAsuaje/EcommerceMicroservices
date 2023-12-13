using Microsoft.EntityFrameworkCore;
using Services.Orders.Domain;
using Services.Orders.Infrastructure;

namespace Services.Orders.Application.Countries;

public interface ICountryService
{
    Task<Result<List<CountryGetDTO>>> GetAllAsync();
}

public class CountryService : ICountryService
{
    private readonly OrderDbContext _context;

    public CountryService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CountryGetDTO>>> GetAllAsync()
        => Result.Ok(
            await _context.ShippingCountries
            .AsNoTracking()
            .Select(x => x.ToGetDTO())
            .ToListAsync()
        );
}
