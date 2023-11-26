namespace Services.Auth.Application;

public record RegistrationRequestDTO(string Email, string FirstName, string LastName, string Password, string? PhoneNumber = null);
