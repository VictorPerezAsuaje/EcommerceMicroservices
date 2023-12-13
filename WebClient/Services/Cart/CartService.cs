using Microsoft.Extensions.Options;

namespace WebClient.Services.Cart;

public interface ICartService
{
    Task<ResponseDTO<List<CartItemGetDTO>>> GetCartByClientIdAsync(Guid clientId);
    Task<ResponseDTO> UpdateCartItemAsync(Guid clientId, Guid productId, CartItemPutDTO dto);
    Task<ResponseDTO> RemoveCartItemAsync(Guid clientId, Guid productId);
    Task<ResponseDTO> ClearCartAsync(Guid clientId);
    Task<ResponseDTO> AddCartItemAsync(Guid clientId, CartItemPostDTO dto);
    Task<ResponseDTO> TransferCartItemsAsync(Guid fromId, Guid toId);
}

public class CartService : ICartService
{
    IBaseService _sender;
    ServiceOptions _cartOptions;

    public CartService(IBaseService sender, IOptions<APIServices> services)
    {
        _sender = sender;
        _cartOptions = services.Value.Cart;
    }

    public async Task<ResponseDTO> AddCartItemAsync(Guid clientId, CartItemPostDTO dto)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = $"{_cartOptions.BaseUrl}/cart/{clientId}",
            Data = dto
        }, false);
    }

    public async Task<ResponseDTO> ClearCartAsync(Guid clientId)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.DELETE,
            Url = $"{_cartOptions.BaseUrl}/cart/{clientId}"
        }, false);
    }

    public async Task<ResponseDTO<List<CartItemGetDTO>>> GetCartByClientIdAsync(Guid clientId)
    {
        return await _sender.SendAsync<List<CartItemGetDTO>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = $"{_cartOptions.BaseUrl}/cart/{clientId}",
        }, false);
    }

    public async Task<ResponseDTO> RemoveCartItemAsync(Guid clientId, Guid productId)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.DELETE,
            Url = $"{_cartOptions.BaseUrl}/cart/{clientId}/{productId}"
        }, false);
    }

    public async Task<ResponseDTO> TransferCartItemsAsync(Guid fromId, Guid toId)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.PUT,
            Url = $"{_cartOptions.BaseUrl}/cart/{fromId}/transfer-to/{toId}"
        }, false);
    }

    public async Task<ResponseDTO> UpdateCartItemAsync(Guid clientId, Guid productId, CartItemPutDTO dto)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.PUT,
            Url = $"{_cartOptions.BaseUrl}/cart/{clientId}/{productId}",
            Data = dto
        }, false);
    }
}
