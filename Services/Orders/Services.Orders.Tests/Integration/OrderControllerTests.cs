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

namespace Services.Orders.Tests.Integration;

[Collection("Order")]
public class OrderControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrderControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetAll_ReturnsOrderList()
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/orders");

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<List<OrderGetDTO>>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var orders = tokenResult.Value;
        orders.Should().OnlyHaveUniqueItems();
        orders.Should().HaveCount(DbInitializer.Orders.Count);
        orders.Should().BeEquivalentTo(DbInitializer.Orders.ToListGetDTO(), 
            config => config.Excluding(x => x.CurrentStatusChangeDate),
            because: "The supported ideas for the datetime testing seems too flaky");

        // Temporary until I get a better idea
        orders.ForEach(x => x.CurrentStatusChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public async Task GetById_WithExistentId_ReturnsOrderRequested()
    {
        // Arrage
        Guid id = DbInitializer.Orders[1].Id;

        // Act
        var result = await _client.GetAsync("/orders/" + id);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<OrderGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var order = tokenResult.Value;
        order.Should().BeEquivalentTo(DbInitializer.Orders[1].ToGetDTO(), config => config.Excluding(x => x.CurrentStatusChangeDate));

        // Temporary until I get a better idea
        order.CurrentStatusChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task GetById_WithUnexistentId_ReturnsNotFound()
    {
        // Arrage
        Guid id = Guid.NewGuid();

        // Act
        var result = await _client.GetAsync("/orders/" + id);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<OrderGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();
        tokenResult.Value.Should().BeNull();
    }


    [Fact]
    public async Task Create_WithMinimumValidData_ReturnsOk()
    {
        // Arrage
        List<OrderItemPostDTO> itemsDto = new()
        {
            new()
            {
                ProductId = DbInitializer.OrderItems[0].ProductId,
                Name = DbInitializer.OrderItems[0].Name,
                Amount = DbInitializer.OrderItems[0].Amount,
                Price = DbInitializer.OrderItems[0].Price
            }
        };

        AddressPostDTO addressDto = new()
        {
            CountryName = DbInitializer.Countries[0].Name,
            CountryCode = DbInitializer.Countries[0].Code,
            MajorDivision = "Test major division",
            Locality = "Test locality",
            Street = "Test street",
            Latitude = 1.2345,
            Longitude = 5.4321
        };

        OrderPostDTO dto = new()
        {
            ClientId = Guid.NewGuid(),
            Items = itemsDto,
            ShippingFirstName = "Test First Name",
            ShippingLastName = "Test Last Name",
            ShippingAddress = addressDto,
            ShippingMethod = DbInitializer.ShippingMethods[0].Name,
            PaymentMethod = DbInitializer.PaymentMethods[0].Name,
        };

        double subtotal = Math.Round(itemsDto.Sum(x => x.Price * x.Amount), 2);
        double total = Math.Round(subtotal + subtotal * DbInitializer.ShippingMethods[0].ApplicableFees, 2);

        // Act
        var result = await _client.PostAsJsonAsync("/orders", dto);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<Guid>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrWhiteSpace();
        tokenResult.Value.Should().NotBe(default(Guid));

        Order? order = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            order = await context.Orders.Include(x => x.ShippingAddress)
                                        .ThenInclude(x => x.Country)
                                        .Include(x => x.Items)    
                                        .Include(x => x.CurrentOrderStatus)
                                        .Include(x => x.History)
                                        .ThenInclude(x => x.Statuses)
                                        .Where(x => x.Id == tokenResult.Value)
                                        .SingleOrDefaultAsync();
        }

        order.Should().NotBeNull();
        order.ClientId.Should().Be(dto.ClientId);
        order.ShippingFirstName.Should().Be(dto.ShippingFirstName);
        order.ShippingLastName.Should().Be(dto.ShippingLastName);

        order.ShippingAddress.Should().NotBeNull();
        order.ShippingAddress.CountryName.Should().Be(addressDto.CountryName);
        order.ShippingAddress.Country.Code.Should().Be(addressDto.CountryCode);
        order.ShippingAddress.MajorDivision.Should().Be(addressDto.MajorDivision);
        order.ShippingAddress.Locality.Should().Be(addressDto.Locality);
        order.ShippingAddress.Street.Should().Be(addressDto.Street);
        order.ShippingAddress.Latitude.Should().Be(addressDto.Latitude);
        order.ShippingAddress.Longitude.Should().Be(addressDto.Longitude);

        order.ShippingMethod.Should().Be(dto.ShippingMethod);
        order.ShippingMethodName.Should().Be(order.ShippingMethod);

        order.PaymentMethodName.Should().Be(dto.PaymentMethod);

        order.DiscountCode.Should().BeNull();
        order.DiscountCodeApplied.Should().BeNull();

        order.Items.Should().NotBeNull().And.HaveCount(1);

        order.CurrentOrderStatus.OrderStatus.Should().Be(OrderStatus.Draft.ToString());
        order.CurrentOrderStatus.ChangeDate.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(10));
        order.CurrentOrderStatus.Message.Should().NotBeNullOrWhiteSpace();

        order.History.Should().NotBeNull();
        order.History.Statuses.Should().HaveCount(1);

        order.SubTotal.Should().Be(subtotal); 
        order.TaxApplied.Should().Be(0.0); 
        order.Total.Should().Be(total); 

        order.OrderDate.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(10)); 
    }

    [Theory]
    [InlineData("", "ES", "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData(" ", "ES", "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData(null, "ES", "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("ThisCountryDoesNotExist", "ES", "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "", "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", " ", "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", null, "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ThisCountryCodeDoesNotExist", "Test Major Division", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", " ", "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", null, "Test locality", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", " ", "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", null, "Test street", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "Test locality", "", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "Test locality", " ", 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "Test locality", null, 2.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "Test locality", "Test street", -91.5465461, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "Test locality", "Test street", 92.0256455, -50.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "Test locality", "Test street", 2.0256455, 90.6545165)]
    [InlineData("Spain", "ES", "Test Major Division", "Test locality", "Test street", 2.0256455, -90.6545165)]
    public async Task Create_WithInvalidAddressData_ReturnsBadRequest(string countryName, string countryCode, string majorDivision, string locality, string street, double latitude, double longitude)
    {
        // Arrage
        List<OrderItemPostDTO> itemsDto = new()
        {
            new()
            {
                ProductId = DbInitializer.OrderItems[0].ProductId,
                Name = DbInitializer.OrderItems[0].Name,
                Amount = DbInitializer.OrderItems[0].Amount,
                Price = DbInitializer.OrderItems[0].Price
            }
        };

        AddressPostDTO addressDto = new()
        {
            CountryName = countryName,
            CountryCode = countryCode,
            MajorDivision = majorDivision,
            Locality = locality,
            Street = street,
            Latitude = latitude,
            Longitude = longitude
        };

        OrderPostDTO dto = new()
        {
            ClientId = Guid.NewGuid(),
            Items = itemsDto,
            ShippingFirstName = "Test First Name",
            ShippingLastName = "Test Last Name",
            ShippingAddress = addressDto,
            ShippingMethod = DbInitializer.ShippingMethods[0].Name,
            PaymentMethod = DbInitializer.PaymentMethods[0].Name,
        };

        // Act
        var result = await _client.PostAsJsonAsync("/orders", dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        Order? order = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            order = await context.Orders.Where(x => x.ClientId == dto.ClientId) // This is the only order that could "match" this case
                                        .SingleOrDefaultAsync();
        }

        order.Should().BeNull();
    }

    [Fact]
    public async Task Create_WithoutItems_ReturnsBadRequest()
    {
        // Arrage
        List<OrderItemPostDTO> itemsDto = new();

        AddressPostDTO addressDto = new()
        {
            CountryName = DbInitializer.Countries[0].Name,
            CountryCode = DbInitializer.Countries[0].Code,
            MajorDivision = "Test major division",
            Locality = "Test locality",
            Street = "Test street",
            Latitude = 1.2345,
            Longitude = 5.4321
        };

        OrderPostDTO dto = new()
        {
            ClientId = Guid.NewGuid(),
            Items = itemsDto,
            ShippingFirstName = "Test First Name",
            ShippingLastName = "Test Last Name",
            ShippingAddress = addressDto,
            ShippingMethod = DbInitializer.ShippingMethods[0].Name,
            PaymentMethod = DbInitializer.PaymentMethods[0].Name,
        };

        // Act
        var result = await _client.PostAsJsonAsync("/orders", dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        Order? order = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            order = await context.Orders.Where(x => x.ClientId == dto.ClientId) // This is the only order that could "match" this case
                                        .SingleOrDefaultAsync();
        }

        order.Should().BeNull();
    }

    [Theory]
    [InlineData("", "Test Last Name", "FedEx", "Stripe")]
    [InlineData(" ", "Test Last Name", "FedEx", "Stripe")]
    [InlineData(null, "Test Last Name", "FedEx", "Stripe")]
    [InlineData("Test First Name", "", "FedEx", "Stripe")]
    [InlineData("Test First Name", " ", "FedEx", "Stripe")]
    [InlineData("Test First Name", null, "FedEx", "Stripe")]
    [InlineData("Test First Name", "Test Last Name", "", "Stripe")]
    [InlineData("Test First Name", "Test Last Name", " ", "Stripe")]
    [InlineData("Test First Name", "Test Last Name", null, "Stripe")]
    [InlineData("Test First Name", "Test Last Name", "FedEx", "")]
    [InlineData("Test First Name", "Test Last Name", "FedEx", " ")]
    [InlineData("Test First Name", "Test Last Name", "FedEx", null)]
    public async Task Create_WithoutRequiredFields_ReturnsBadRequest(string firstName, string lastName, string shippingMethod, string paymentMethod)
    {
        // Arrage
        List<OrderItemPostDTO> itemsDto = new()
        {
            new()
            {
                ProductId = DbInitializer.OrderItems[0].ProductId,
                Name = DbInitializer.OrderItems[0].Name,
                Amount = DbInitializer.OrderItems[0].Amount,
                Price = DbInitializer.OrderItems[0].Price
            }
        };

        AddressPostDTO addressDto = new()
        {
            CountryName = DbInitializer.Countries[0].Name,
            CountryCode = DbInitializer.Countries[0].Code,
            MajorDivision = "Test major division",
            Locality = "Test locality",
            Street = "Test street",
            Latitude = 1.2345,
            Longitude = 5.4321
        };

        OrderPostDTO dto = new()
        {
            ClientId = Guid.NewGuid(),
            Items = itemsDto,
            ShippingFirstName = firstName,
            ShippingLastName = lastName,
            ShippingAddress = addressDto,
            ShippingMethod = shippingMethod,
            PaymentMethod = paymentMethod,
        };

        double subtotal = Math.Round(itemsDto.Sum(x => x.Price * x.Amount), 2);
        double total = Math.Round(subtotal + subtotal * DbInitializer.ShippingMethods[0].ApplicableFees, 2);

        // Act
        var result = await _client.PostAsJsonAsync("/orders", dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        Order? order = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            order = await context.Orders.Where(x => x.ClientId == dto.ClientId) // This is the only order that could "match" this case
                                        .SingleOrDefaultAsync();
        }

        order.Should().BeNull();
    }

    [Fact]
    public async Task CancelOrder_ExistingId_ReturnsOkAndChangesStatus()
    {
        // Arrage
        Guid id = DbInitializer.Orders[1].Id;

        // Act
        var result = await _client.PutAsync("/orders/" + id + "/cancel", null);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        Order? order = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            order = await context.Orders.Include(x => x.CurrentOrderStatus)
                                        .Include(x => x.History)
                                        .ThenInclude(x => x.Statuses)
                                        .Where(x => x.Id == id) 
                                        .SingleOrDefaultAsync();
        }

        order.Should().NotBeNull();
        order.CurrentOrderStatus.ChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        order.CurrentOrderStatus.Message.Should().Be("The cancel request has been applied.");
        order.CurrentOrderStatus.OrderStatus.Should().Be(OrderStatus.Cancelled.ToString());
        order.History.Statuses.Should().HaveCount(2);

        var cancelledStatus = order.History.Statuses.OrderByDescending(x => x.ChangeDate).First();
        cancelledStatus.Should().BeEquivalentTo(order.CurrentOrderStatus);
    }

    [Fact]
    public async Task CancelOrder_NotExistingId_ReturnsNotFound()
    {
        // Arrage
        Guid id = Guid.NewGuid();

        // Act
        var result = await _client.PutAsync("/orders/" + id + "/cancel", null);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        Order? order = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            order = await context.Orders.Where(x => x.Id == id).SingleOrDefaultAsync();
        }

        order.Should().BeNull();
    }
}
