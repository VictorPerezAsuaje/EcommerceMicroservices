namespace WebClient.Services;

public enum EndpointType { GET, POST, PUT, DELETE }

public class RequestDTO {
    public EndpointType EndpointType { get; set; } = EndpointType.GET;
    public string Url { get; set; }
    public object Data { get; set; }
    public string AccessToken { get; set; }
}