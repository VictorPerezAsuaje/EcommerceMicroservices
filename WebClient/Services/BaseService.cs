using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebClient.Services.Auth;

namespace WebClient.Services;

public interface IBaseService
{
    Task<ResponseDTO> SendAsync(RequestDTO request, bool requiresAuth = true);
    Task<ResponseDTO<T>?> SendAsync<T>(RequestDTO request, bool requiresAuth = true);
}
public class BaseService : IBaseService
{
    IHttpClientFactory _httpClientFactory;
    ITokenProvider _tokenProvider;

    public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
    }



    public async Task<ResponseDTO<T>?> SendAsync<T>(RequestDTO request, bool requiresAuth = true)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MicroEcom");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            if(requiresAuth)
            {
                var token = _tokenProvider.GetToken();

                if (token is null)
                    return new ResponseDTO<T>(false, default, "You need to log in first to complete your request.");

                message.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {token}");
            }

            message.RequestUri = new Uri(request.Url);

            if (request.Data is not null)
                message.Content = new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json");

            message.Method = request.EndpointType switch
            {
                EndpointType.POST => HttpMethod.Post,
                EndpointType.PUT => HttpMethod.Put,
                EndpointType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get,
            };

            HttpResponseMessage? response = await client.SendAsync(message);
            return await response.Content.ReadFromJsonAsync<ResponseDTO<T>>();
        }
        catch (Exception ex)
        {
            return new(false, default(T), "There has been an error during the request.");
        }        
    }

    public async Task<ResponseDTO> SendAsync(RequestDTO request, bool requiresAuth = true)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MicroEcom");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            if (requiresAuth)
            {
                var token = _tokenProvider.GetToken();

                if (token is null)
                    return new ResponseDTO(false, "You need to log in first to complete your request.");

                message.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {token}");
            }

            message.RequestUri = new Uri(request.Url);

            if (request.Data is not null)
                message.Content = new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json");

            message.Method = request.EndpointType switch
            {
                EndpointType.POST => HttpMethod.Post,
                EndpointType.PUT => HttpMethod.Put,
                EndpointType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get,
            };

            HttpResponseMessage? response = await client.SendAsync(message);
            return await response.Content.ReadFromJsonAsync<ResponseDTO>();
        }
        catch (Exception ex)
        {
            return new(false, "There has been an error during the request.");
        }
    }
}
