using Services.Auth.Domain;

namespace Services.Auth.Application;

public static class ResultExtensions
{
    public static ResponseDTO ToResponseDTO(this Result result)
        => new ResponseDTO(result.IsSuccess, result.Error);

    public static ResponseDTO ToResponseDTO<T>(this Result<T> result) where T : class
        => new ResponseDTO<T>(result.IsSuccess, result.IsSuccess ? result.Value : null, result.Error);
}

public record ResponseDTO (bool IsSuccess, string? Error = null) 
{
    public bool IsFailure => !IsSuccess;
};

public record ResponseDTO<T>(bool IsSuccess, T Value, string? Error = null) : ResponseDTO(IsSuccess, Error)
{
    public bool IsFailure => !IsSuccess;
};