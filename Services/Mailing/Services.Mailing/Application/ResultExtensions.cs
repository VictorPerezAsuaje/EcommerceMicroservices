using Services.Mailing.Domain;

namespace Services.Mailing.Application
{
    public static class ResultExtensions
    {
        public static ResponseDTO ToResponseDTO(this Result result)
            => new ResponseDTO(result.IsSuccess, result.Error);

        public static ResponseDTO<T> ToResponseDTO<T>(this Result<T> result)
            => new ResponseDTO<T>(result.IsSuccess, result.IsSuccess ? result.Value : default(T), result.Error);
    }
}
