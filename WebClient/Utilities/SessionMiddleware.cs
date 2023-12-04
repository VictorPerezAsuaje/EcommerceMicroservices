using System.IdentityModel.Tokens.Jwt;

namespace WebClient.Utilities;

public class SessionMiddleware
{
    private readonly RequestDelegate _next;

    public SessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Cookies.ContainsKey("SessionId"))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            string? clientId = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            context.Response.Cookies.Append("SessionId", clientId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMonths(1),
                HttpOnly = true,
            });

            await _next(context);
            return;
        }

        string sessionId = Guid.NewGuid().ToString();
        context.Response.Cookies.Append("SessionId", sessionId, new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddMonths(1),
            HttpOnly = true,
        });        

        await _next(context);
    }
}
