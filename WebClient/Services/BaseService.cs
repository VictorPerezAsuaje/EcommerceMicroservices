using System.Net;
using System.Text;
using System.Text.Json;

namespace WebClient.Services;

public interface IBaseService
{
    Task<ResponseDTO<T>?> SendAsync<T>(RequestDTO request);
}
public class BaseService : IBaseService
{
    IHttpClientFactory _httpClientFactory;

    public BaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ResponseDTO<T>?> SendAsync<T>(RequestDTO request)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MicroEcom");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            // Access Token for Auth here

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
}
