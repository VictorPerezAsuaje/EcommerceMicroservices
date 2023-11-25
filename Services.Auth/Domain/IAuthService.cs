using Services.Auth.Application;

namespace Services.Auth.Domain;

public interface IAuthService
{
    Task<Result> Register(RegistrationRequestDTO request);
    Task<Result> Login(LoginRequestDTO request);
}