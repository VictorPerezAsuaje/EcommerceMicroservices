using Microsoft.EntityFrameworkCore;
using Services.Cart.Application;
using Services.Cart.Domain;
using Services.Cart.Infrastructure;

namespace Services.Cart.Application;

public interface ICartService
{
    Task<Result<List<CartItemGetDTO>>> GetCartByClientIdAsync(Guid clientId);
    Task<Result> UpdateCartItemAsync(Guid clientId, Guid productId, CartItemPutDTO dto);
    Task<Result> RemoveCartItemAsync(Guid clientId, Guid productId);
    Task<Result> AddCartItemAsync(Guid clientId, CartItemPostDTO dto);
}

public class CartService : ICartService
{
    private readonly CartDbContext _context;

    public CartService(CartDbContext context)
    {
        _context = context;
    }

    public async Task<Result> AddCartItemAsync(Guid clientId, CartItemPostDTO dto)
    {
        bool idAlreadyExists = await _context.CartItems
                                        .AnyAsync(x => x.ClientId == clientId && x.ProductId == dto.ProductId);

        if (idAlreadyExists)
            return Result.Fail<Guid>("That product already exists in your cart.");

        CartItem item = dto.ToCartItem(clientId);

        await _context.CartItems.AddAsync(item);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> RemoveCartItemAsync(Guid clientId, Guid productId)
    {
        CartItem? item = await _context.CartItems
                                        .Where(x => x.ClientId == clientId && x.ProductId == productId)
                                        .FirstOrDefaultAsync();

        if (item is null)
            return Result.Fail("The selected product to delete does not exist in your cart.");

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> UpdateCartItemAsync(Guid clientId, Guid productId, CartItemPutDTO dto)
    {
        CartItem? item = await _context.CartItems
                                        .Where(x => x.ClientId == clientId && x.ProductId == productId)
                                        .FirstOrDefaultAsync();

        if (item is null)
            return Result.Fail("The selected product to update does not exist.");

        _context.CartItems.Update(item);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<List<CartItemGetDTO>>> GetCartByClientIdAsync(Guid clientId)
        => Result.Ok(await _context.CartItems
                                            .Where(x => x.ClientId == clientId)
                                            .ToQueryableGetDTO()
                                            .ToListAsync());
}
