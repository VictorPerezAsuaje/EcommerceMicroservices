using Services.Cart.Application;
using Services.Cart.Domain;

namespace Services.Cart.Application;

public static class CartItemExtensions
{
    public static CartItem ToCartItem(this CartItemPostDTO dto, Guid clientId)
        => new CartItem(clientId, dto.ProductId, dto.ThumbnailUrl, dto.Name, dto.Price, dto.Amount)
                    .ApplyDiscount(dto.DiscountApplied);

    public static List<CartItemGetDTO> ToListGetDTO(this IEnumerable<CartItem> CartItems)
        => CartItems.Select(ToGetDTO).ToList();

    public static IQueryable<CartItemGetDTO> ToQueryableGetDTO(this IQueryable<CartItem> query)
        => query.Select(x => new CartItemGetDTO
        {
            ProductId = x.ProductId,
            Name = x.Name,
            Price = x.Price,
            ThumbnailUrl = x.ThumbnailUrl,
            Amount = x.Amount,
            DiscountApplied = x.DiscountApplied,
            ComputedPrice = x.ComputedPrice,
            IsFree = x.IsFree
        });

    public static CartItemGetDTO ToGetDTO(this CartItem cartItem)
        => new CartItemGetDTO()
        {
            ProductId = cartItem.ProductId,
            Name = cartItem.Name,
            Price = cartItem.Price,
            ThumbnailUrl = cartItem.ThumbnailUrl,
            Amount = cartItem.Amount,
            DiscountApplied = cartItem.DiscountApplied,
            ComputedPrice = cartItem.ComputedPrice,
            IsFree = cartItem.IsFree
        };
}