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

        Result<Address> addressResult = dto.ShippingAddress.ToAddress();

        if (addressResult.IsFailure)
            return Result.Fail<Guid>(addressResult.Error);

        Address address = addressResult.Value;

        ShippingMethod? shippingMethod = await _context.ShippingMethods
            .Where(x => x.Name == dto.ShippingMethod && x.CountryName == address.CountryName)
            .SingleOrDefaultAsync();

        if (shippingMethod is null)
            return Result.Fail<Guid>("Order could not be created, selected shipping method could not be found for that country.");

        DiscountCode? discount = await _context.DiscountCodes
            .Where(x => x.Code == dto.DiscountCodeApplied)
            .SingleOrDefaultAsync();

        if (discount is null)
            return Result.Fail<Guid>("Order could not be created, selected discount could not be found.");

        Result<Order> orderResult = Order.Generate
            (
                orderId: newOrderId,
                clientId: dto.ClientId,
                firstName: dto.ShippingFirstName,
                lastName: dto.ShippingLastName,
                shippingAddress: address,
                shippingMethod: shippingMethod,
                shippingFees: shippingMethod.ApplicableFees,
                items: dto.Items.ToListOrderItem(newOrderId),
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
        Order? order = await _context.Orders.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (order is null)
            return Result.Fail("The selected order to cancel does not exist.");

        order.CancelOrder();
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<List<OrderGetDTO>>> GetAllAsync()
        => Result.Ok(await _context.Orders.ToQueryableGetDTO().ToListAsync());

    public async Task<Result<OrderGetDTO>> GetByIdAsync(Guid id)
    {
        OrderGetDTO? order = await _context.Orders
                                    .Where(x => x.Id == id)
                                    .ToQueryableGetDTO()
                                    .FirstOrDefaultAsync();

        if (order is null)
            return Result.Fail<OrderGetDTO>("The order could not be found.");

        return Result.Ok(order);
    }
}
