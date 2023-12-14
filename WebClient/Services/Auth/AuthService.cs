using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebClient.Services.Catalog.Tags;

namespace WebClient.Services.Auth;

public interface IAuthService
{
    Task<ResponseDTO<string>> LoginAsync(LoginPostDTO dto);
    Task<ResponseDTO<ChallengeResult>> GoogleLoginAsync(string provider, string? redirectUrl = null);
    Task<ResponseDTO> RegisterAsync(RegisterPostDTO dto);
    Task<ResponseDTO> RecoverPasswordAsync(ForgotPasswordPostDTO dto);
    Task<ResponseDTO<IEnumerable<AuthSchemeDTO>>> GetExternalAuthProviders();
    Task<ResponseDTO<string>> CompleteGoogleLogin(ExternalLoginDataDTO dto);
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

    public async Task<ResponseDTO<IEnumerable<AuthSchemeDTO>>> GetExternalAuthProviders()
    {
        return await _sender.SendAsync<IEnumerable<AuthSchemeDTO>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = $"{_authOptions.BaseUrl}/auth/login/external-auth-providers",
        }, false);
    }

    public async Task<ResponseDTO<string>> CompleteGoogleLogin(ExternalLoginDataDTO dto)
    {
        return await _sender.SendAsync<string>(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = $"{_authOptions.BaseUrl}/auth/login/complete-google-login",
            Data = dto
        }, false);
    }

    public async Task<ResponseDTO<ChallengeResult>> GoogleLoginAsync(string provider, string? redirectUrl = null)
    {
        return await _sender.SendAsync<ChallengeResult>(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = $"{_authOptions.BaseUrl}/auth/login/google",
            Data = new { provider = provider,  redirectUrl = redirectUrl } 
        }, false);
    }

    public async Task<ResponseDTO<string>> LoginAsync(LoginPostDTO dto)
    {
        return await _sender.SendAsync<string>(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = $"{_authOptions.BaseUrl}/auth/login",
            Data = dto
        }, false);
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
            Url = $"{_authOptions.BaseUrl}/auth/register",
            Data = dto
        }, false);
    }
}