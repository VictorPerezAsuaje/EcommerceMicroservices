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
using Services.Orders.Application.PaymentMethods;
namespace Services.Orders.Tests.Integration;

[Collection("Order")]
public class PaymentMethodControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public PaymentMethodControllerTests(CustomWebApplicationFactory factory)
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
        var result = await _client.GetAsync("/payment-methods");

        // Assert
        result.EnsureSuccessStatusCode();

        var response = await result.Content.ReadFromJsonAsync<ResponseDTO<List<PaymentMethodGetDTO>>>();
        response.Should().NotBeNull();
        response.IsSuccess.Should().Be(true);
        response.Error.Should().BeNullOrEmpty();

        var countries = response.Value;
        countries.Should().OnlyHaveUniqueItems();
        countries.Should().BeEquivalentTo(PaymentMethod.GetAll().ToListGetDTO());

    }
}

