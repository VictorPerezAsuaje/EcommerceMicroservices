namespace WebClient.Services;

public record ResponseDTO(bool IsSuccess, string? Error = null)
{
    public bool IsFailure => !IsSuccess;
};

public record ResponseDTO<T>(bool IsSuccess, T? Value, string? Error = null) : ResponseDTO(IsSuccess, Error)
{
    public bool IsFailure => !IsSuccess;
};
