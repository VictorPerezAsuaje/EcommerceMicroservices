namespace WebClient.Services.Auth;


public interface ITokenProvider
{
    void ClearToken();
    string? GetToken();
    void SetToken(string token);
}

public class TokenProvider : ITokenProvider
{
    IHttpContextAccessor _contextAccessor;

    public TokenProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void ClearToken()
    {
        _contextAccessor.HttpContext?.Response.Cookies.Delete(APIServices.AuthCookie);
    }

    public string? GetToken()
    {
        string? token = null;
        bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(APIServices.AuthCookie, out token);
        return hasToken is true ? token : null;
    }

    public void SetToken(string token)
    {
        _contextAccessor.HttpContext?.Response.Cookies.Append(APIServices.AuthCookie, token);
    }
}
