namespace Services.Auth.Application;

public class GoogleLoginPostDTO
{
    public string Provider { get; set; }
    public string? RedirectUrl { get; set; }
}
