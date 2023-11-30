using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Services.Auth.Application;
using Services.Auth.Domain;
using Services.Auth.Infrastructure;
using Services.Auth.Tests.Utilities;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Services.Auth.Tests.Integration;

[CollectionDefinition("Auth")]
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

        factory.CleanupDatabase();
    }

    [Theory]
    [InlineData("client@client", "C1ientP@ssw0rd")]
    [InlineData("admin@admin", "Adm1nP@ssw0rd")]
    public async Task Login_WithValidCredentials_ReturnsToken(string username, string password)
    {
        // Arrage
        LoginRequestDTO dto = new LoginRequestDTO(username, password);

        // Act
        var result = await _client.PostAsJsonAsync("/auth/login", dto);

        // Assert
        result.EnsureSuccessStatusCode();
        
        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<string>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();
        tokenResult.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("client@client", "")]
    [InlineData("client@client", "IncorrectPassword")]
    [InlineData("", "C1ientP@ssw0rd")]
    [InlineData("IncorrectUsername", "C1ientP@ssw0rd")]
    public async Task Login_WithInvalidCredentials_ReturnsError(string username, string password)
    {
        // Arrage
        LoginRequestDTO dto = new LoginRequestDTO(username, password);

        // Act
        var result = await _client.PostAsJsonAsync("/auth/login", dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<string>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Value.Should().BeNull();
        tokenResult.Error.Should().NotBeNullOrEmpty();
        tokenResult.Error.Should().Be("Invalid credentials");
    }

    [Theory]
    [InlineData("client.full@client", "Client", "Two", "123 123 123", "Va1idP@ssw0rd")]
    [InlineData("client.nullphone@client", "Client", "Three", null, "Va1idP@ssw0rd")]
    [InlineData("client.emptyphone@client", "Client", "Four", "", "Va1idP@ssw0rd")]
    public async Task Register_WithValidCredentials_ReturnsOk(string email, string firstName, string lastName, string phone, string password)
    {
        // Arrage
        RegistrationRequestDTO dto = new RegistrationRequestDTO(email, firstName, lastName, password, phone);

        // Act
        var result = await _client.PostAsJsonAsync("/auth/register", dto);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("client@client", "Existing", "Client", "Va1idP@ssw0rd")]
    public async Task Register_WithExistingEmail_ReturnsError(string email, string firstName, string lastName, string password)
    {
        // Arrage
        RegistrationRequestDTO dto = new RegistrationRequestDTO(email, firstName, lastName, password);

        // Act
        var result = await _client.PostAsJsonAsync("/auth/register", dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrEmpty();
        tokenResult.Error.Should().Be("The email used to create the account already exists");
    }

    [Theory]
    [InlineData("", "Client", "Two", "Va1idP@ssw0rd")]
    [InlineData("client2@client", "", "Two", "Va1idP@ssw0rd")]
    [InlineData("client2@client", "Client", "", "Va1idP@ssw0rd")]
    public async Task Register_WithInvalidCredentials_ReturnsError(string email, string firstName, string lastName, string password)
    {
        // Arrage
        RegistrationRequestDTO dto = new RegistrationRequestDTO(email, firstName, lastName, password);

        // Act
        var result = await _client.PostAsJsonAsync("/auth/register", dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrEmpty();
        tokenResult.Error.Should().Be("There were validation errors trying to create the user.");
    }

    [Theory]
    [InlineData("client2@client", "Client", "Two", "")]
    [InlineData("client2@client", "Client", "Two", "0nlylower$")]
    [InlineData("client2@client", "Client", "Two", "0NLYUPPER$")]
    [InlineData("client2@client", "Client", "Two", "NoNumberHere$")]
    [InlineData("client2@client", "Client", "Two", "NoUn1queChars")]
    public async Task Register_WithInvalidPasswords_ReturnsError(string email, string firstName, string lastName, string password)
    {
        // Arrage
        RegistrationRequestDTO dto = new RegistrationRequestDTO(email, firstName, lastName, password);

        // Act
        var result = await _client.PostAsJsonAsync("/auth/register", dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrEmpty();
    }
}
