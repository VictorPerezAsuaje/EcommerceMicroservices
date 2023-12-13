using Microsoft.Extensions.Options;
using WebClient.Services.Orders.ApiDTOS;
using WebClient.Services.Orders.ViewModels;

namespace WebClient.Services.Order;

public interface IOrderService
{
    Task<ResponseDTO<List<CountryVM>>> GetAvailableCoutriesAsync();
    Task<ResponseDTO<List<ShippingMethodVM>>> GetAvailableShippingMethodsAsync(string countryName);
    Task<ResponseDTO<List<PaymentMethodVM>>> GetAvailablePaymentMethodsAsync();
    Task<ResponseDTO> CancelOrderAsync(Guid clientId);
    Task<ResponseDTO> PlaceOrderAsync(Guid clientId, OrderPostDTO dto);
}

public class OrderService : IOrderService
{
    IBaseService _sender;
    ServiceOptions _orderOptions;

    public OrderService(IBaseService sender, IOptions<APIServices> services)
    {
        _sender = sender;
        _orderOptions = services.Value.Order;
    }

    public async Task<ResponseDTO> PlaceOrderAsync(Guid clientId, OrderPostDTO dto)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = $"{_orderOptions.BaseUrl}/orders",
            Data = dto
        });
    }

    public async Task<ResponseDTO> CancelOrderAsync(Guid clientId)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.PUT,
            Url = $"{_orderOptions.BaseUrl}/orders/{clientId}/cancel"
        });
    }

    public async Task<ResponseDTO<List<CountryVM>>> GetAvailableCoutriesAsync()
    {
        return await _sender.SendAsync<List<CountryVM>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = $"{_orderOptions.BaseUrl}/countries"
        });
    }

    public async Task<ResponseDTO<List<ShippingMethodVM>>> GetAvailableShippingMethodsAsync(string countryName)
    {
        return await _sender.SendAsync<List<ShippingMethodVM>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = $"{_orderOptions.BaseUrl}/shipping-methods/{countryName}"
        });
    }

    public async Task<ResponseDTO<List<PaymentMethodVM>>> GetAvailablePaymentMethodsAsync()
    {
        return await _sender.SendAsync<List<PaymentMethodVM>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = $"{_orderOptions.BaseUrl}/payment-methods"
        });
    }
}
