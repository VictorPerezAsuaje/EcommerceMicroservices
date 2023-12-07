using Microsoft.EntityFrameworkCore;
using Services.Orders.Domain;
using Services.Orders.Infrastructure;

namespace Services.Orders.Application.Orders;

public interface IOrderService
{
    Task<Result<OrderGetDTO>> GetByIdAsync(Guid id);
    Task<Result<List<OrderGetDTO>>> GetAllAsync();
    Task<Result> CancelOrderAsync(Guid id);
    Task<Result<Guid>> CreateAsync(OrderPostDTO order);
}

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;

    public OrderService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> CreateAsync(OrderPostDTO dto)
    {
        Guid newOrderId = Guid.NewGuid();

        bool isSupportedCountry = await _context.ShippingCountries.AnyAsync(x => x.Name == dto.ShippingAddress.CountryName && x.Code == dto.ShippingAddress.CountryCode);

        if (!isSupportedCountry)
            return Result.Fail<Guid>("The order could not be created because the country selected is not supported in our system.");

        Result<Address> addressResult = dto.ShippingAddress.ToAddress();

        if (addressResult.IsFailure)
            return Result.Fail<Guid>(addressResult.Error);

        Address address = addressResult.Value;

        ShippingMethod? shippingMethod = await _context.ShippingMethods
            .Where(x => x.Name == dto.ShippingMethod && x.CountryName == address.CountryName)
            .SingleOrDefaultAsync();

        if (shippingMethod is null)
            return Result.Fail<Guid>("Order could not be created, selected shipping method could not be found for that country.");

        DiscountCode? discount = null;
        
        if(dto.DiscountCodeApplied != null)
        {
            discount = await _context.DiscountCodes
                .Where(x => x.Code == dto.DiscountCodeApplied)
                .SingleOrDefaultAsync();

            if (discount is null)
                return Result.Fail<Guid>("Order could not be created, selected discount could not be found.");
        }
        

        PaymentMethod? paymentMethod = await _context.PaymentMethods
            .Where(x => x.Name == dto.PaymentMethod)
            .SingleOrDefaultAsync();

        if (paymentMethod is null)
            return Result.Fail<Guid>("Order could not be created, selected payment method could not be found.");

        Result<Order> orderResult = Order.Generate
            (
                orderId: newOrderId,
                clientId: dto.ClientId,
                firstName: dto.ShippingFirstName,
                lastName: dto.ShippingLastName,
                shippingAddress: address,
                shippingMethod: shippingMethod,
                items: dto.Items.ToListOrderItem(newOrderId),
                paymentMethod: paymentMethod,
                taxApplied: 0.0,
                discountCode: discount
            );

        if(orderResult.IsFailure)
            return Result.Fail<Guid>(orderResult.Error);

        await _context.Orders.AddAsync(orderResult.Value);
        await _context.SaveChangesAsync();
        return Result.Ok(newOrderId);
    }

    public async Task<Result> CancelOrderAsync(Guid id)
    {
        Order? order = await _context.Orders.Where(x => x.Id == id).Include(x => x.History).FirstOrDefaultAsync();

        if (order is null)
            return Result.Fail("The selected order to cancel does not exist.");

        order.CancelOrder();
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<List<OrderGetDTO>>> GetAllAsync()
        => Result.Ok(
            await _context.Orders
            .AsNoTracking()
            .ToQueryableGetDTO()
            .ToListAsync()
        );

    public async Task<Result<OrderGetDTO>> GetByIdAsync(Guid id)
    {
        OrderGetDTO? order = await _context.Orders
                                    .Where(x => x.Id == id)
                                    .AsNoTracking()
                                    .ToQueryableGetDTO()
                                    .FirstOrDefaultAsync();

        if (order is null)
            return Result.Fail<OrderGetDTO>("The order could not be found.");

        return Result.Ok(order);
    }
}
