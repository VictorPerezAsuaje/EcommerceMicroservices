using Microsoft.EntityFrameworkCore;
using Services.Cart.Application;
using Services.Cart.Domain;
using Services.Cart.Infrastructure;

namespace Services.Cart.Application;

public interface ICartService
{
    Task<Result<List<CartItemGetDTO>>> GetCartByClientIdAsync(Guid clientId);
    Task<Result> UpdateCartItemAmountAsync(Guid clientId, Guid productId, ItemAmountPutDTO dto);
    Task<Result> TransferCartItemsAsync(Guid fromId, Guid toId);
    Task<Result> RemoveCartItemAsync(Guid clientId, Guid productId);
    Task<Result> ClearCartAsync(Guid clientId);
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
        CartItem? item = await _context.CartItems.SingleOrDefaultAsync(x => x.ClientId == clientId && x.ProductId == dto.ProductId);

        if (item is not null)
        {
            item.IncreaseAmount(dto.Amount);
            _context.CartItems.Update(item.RenewExpirationTime());
        }
        else
        {
            item = dto.ToCartItem(clientId);
            await _context.CartItems.AddAsync(item.RenewExpirationTime());
        }
         
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

    public async Task<Result> UpdateCartItemAmountAsync(Guid clientId, Guid productId, ItemAmountPutDTO dto)
    {
        CartItem? item = await _context.CartItems.FirstOrDefaultAsync(x => x.ClientId == clientId && x.ProductId == productId);

        if (item is null)
            return Result.Fail("The selected product to update does not exist.");

        if(dto.Amount is not null)
            item.UpdateAmount(dto.Amount.Value);

        _context.CartItems.Update(item.RenewExpirationTime());
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<List<CartItemGetDTO>>> GetCartByClientIdAsync(Guid clientId)
        => Result.Ok(await _context.CartItems
                                            .Where(x => x.ClientId == clientId)
                                            .ToQueryableGetDTO()
                                            .ToListAsync());

    public async Task<Result> TransferCartItemsAsync(Guid fromId, Guid toId)
    {
        var destinyProducts = await _context.CartItems
                                                .Where(x => x.ClientId == toId)
                                                .Select(x => x.ProductId)
                                                .ToListAsync();

        // Get the items that do not exist on the destiny 
        var unexistentItems = await _context.CartItems
            .Where(x => x.ClientId == fromId)
            .Where(x => !destinyProducts.Contains(x.ProductId))
            .Select(x => new CartItem(toId, x.ProductId, x.ThumbnailUrl, x.Name, x.Price, x.Amount).RenewExpirationTime())
            .ToListAsync();

        if (unexistentItems.Count == 0)
            return Result.Ok();

        await _context.CartItems.AddRangeAsync(unexistentItems);        

        var originItems = await _context.CartItems.Where(x => x.ClientId == fromId).ToListAsync();
        _context.CartItems.RemoveRange(originItems);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ClearCartAsync(Guid clientId)
    {
        var items = await _context.CartItems
                                        .Where(x => x.ClientId == clientId)
                                        .ToListAsync();

        if (items.Count == 0)
            return Result.Ok();

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }
}
