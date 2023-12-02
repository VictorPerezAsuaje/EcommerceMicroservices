using Microsoft.Extensions.Options;
using WebClient.Services.Catalog.Tags;

namespace WebClient.Services.Auth;

public interface IAuthService
{
    Task<ResponseDTO<string>> LoginAsync(LoginPostDTO dto);
    Task<ResponseDTO> RegisterAsync(RegisterPostDTO dto);
    Task<ResponseDTO> RecoverPasswordAsync(ForgotPasswordPostDTO dto);
}

public class AuthService : IAuthService
{
    IBaseService _sender;
    ServiceOptions _authOptions;

    public AuthService(IBaseService sender, IOptions<APIServices> services)
    {
        _sender = sender;
        _authOptions = services.Value.Auth;
    }

    public async Task<ResponseDTO<string>> LoginAsync(LoginPostDTO dto)
    {
        return await _sender.SendAsync<string>(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = _authOptions.BaseUrl + "/auth/login",
            Data = dto
        });
    }

    public async Task<ResponseDTO> RecoverPasswordAsync(ForgotPasswordPostDTO dto)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDTO> RegisterAsync(RegisterPostDTO dto)
    {
        return await _sender.SendAsync(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = _authOptions.BaseUrl + "/auth/register",
            Data = dto
        });
    }
}