using Microsoft.AspNetCore.DataProtection;

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
        CookieOptions cookieOptions = new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            Secure = true,
            IsEssential = true
        };

        _contextAccessor.HttpContext?.Response.Cookies.Delete(APIServices.AuthCookie, cookieOptions);        
    }

    public string? GetToken()
    {
        string? token = null;
        bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(APIServices.AuthCookie, out token);
        return hasToken is true ? token : null;
    }

    public void SetToken(string token)
    {
        CookieOptions cookieOptions = new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddMonths(1),
            Secure = true,
            IsEssential = true
        };

        _contextAccessor.HttpContext?.Response.Cookies.Append(APIServices.AuthCookie, token, cookieOptions);
    }
}
