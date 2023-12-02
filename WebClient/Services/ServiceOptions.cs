namespace WebClient.Services;

public class APIServices
{
    public const string AuthCookie = "AuthCookie";
    public ServiceOptions Auth { get; set; }
    public ServiceOptions Catalog { get; set; }
}

public class ServiceOptions
{
    public string Version { get; set; }
    public string BaseUrl { get; set; }
}
