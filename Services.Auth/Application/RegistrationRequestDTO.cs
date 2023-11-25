namespace Services.Auth.Application;

public record RegistrationRequestDTO(string Email, string FirstName, string LastName, string PhoneNumber, string Password);
