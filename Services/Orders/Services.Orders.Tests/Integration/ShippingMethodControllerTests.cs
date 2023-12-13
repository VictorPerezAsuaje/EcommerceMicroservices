using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Orders.Tests.Utilities;
using System.Net;
using System.Net.Http.Json;
using Services.Orders.Application;
using Services.Orders.Application.Orders;
using Services.Orders.Domain;
using Services.Orders.Infrastructure;
using Xunit.Abstractions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Services.Orders.Application.Countries;
using Services.Orders.Application.ShippingMethods;
using Services.Orders.Application.PaymentMethods;

namespace Services.Orders.Tests.Integration;

[Collection("Order")]
public class ShippingMethodControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ShippingMethodControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetAll_WithExistingCountryName_ReturnsOk()
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/shipping-methods/spain");

        // Assert
        result.EnsureSuccessStatusCode();

        var response = await result.Content.ReadFromJsonAsync<ResponseDTO<List<ShippingMethodGetDTO>>>();
        response.Should().NotBeNull();
        response.IsSuccess.Should().Be(true);
        response.Error.Should().BeNullOrEmpty();

        var countries = response.Value;
        countries.Should().OnlyHaveUniqueItems();
        countries.Should().BeEquivalentTo(DbInitializer.ShippingMethods.Where(x => x.Countries.Any(x => x.Name == "Spain")).ToListGetDTO());
    }
}
