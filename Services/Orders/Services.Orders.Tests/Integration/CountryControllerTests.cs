using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Services.Orders.Tests.Utilities;
using System.Net.Http.Json;
using Services.Orders.Application;
using Services.Orders.Application.Orders;
using System.Net.Http.Headers;
using Services.Orders.Application.Countries;

namespace Services.Orders.Tests.Integration;

[Collection("Order")]
public class CountryControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CountryControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

        factory.CleanupDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/countries");

        // Assert
        result.EnsureSuccessStatusCode();

        var response = await result.Content.ReadFromJsonAsync<ResponseDTO<List<CountryGetDTO>>>();
        response.Should().NotBeNull();
        response.IsSuccess.Should().Be(true);
        response.Error.Should().BeNullOrEmpty();

        var countries = response.Value;
        countries.Should().OnlyHaveUniqueItems();
        countries.Should().BeEquivalentTo(DbInitializer.Countries.ToListGetDTO());

    }
}
