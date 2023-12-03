using Services.Cart.Domain;

namespace Services.Cart.Application;

public static class ResultExtensions
{
    public static ResponseDTO ToResponseDTO(this Result result)
        => new ResponseDTO(result.IsSuccess, result.Error);

    public static ResponseDTO<T> ToResponseDTO<T>(this Result<T> result)
        => new ResponseDTO<T>(result.IsSuccess, result.IsSuccess ? result.Value : default(T), result.Error);
}

public record ResponseDTO(bool IsSuccess, string? Error = null)
{
    public bool IsFailure => !IsSuccess;
};

public record ResponseDTO<T>(bool IsSuccess, T? Value, string? Error = null) : ResponseDTO(IsSuccess, Error)
{
    public bool IsFailure => !IsSuccess;
};