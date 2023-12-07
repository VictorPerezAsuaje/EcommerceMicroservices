using Azure.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Cart.Application;
using Services.Cart.Domain;
using Services.Cart.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Services.Cart.Tests.Integration;

[Collection("Cart")]
public class CartControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CartControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        factory.CleanupDatabase();
    }

    [Fact]
    public async Task GetCart_WithValidClientId_ReturnsCartItemList()
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/cart/" + DbInitializer.ClientIdOne);

        // Assert
        result.EnsureSuccessStatusCode();

        var list = await result.Content.ReadFromJsonAsync<ResponseDTO<List<CartItemGetDTO>>>();
        list.Should().NotBeNull();
        list.IsSuccess.Should().Be(true);
        list.Error.Should().BeNullOrEmpty();

        list.Value.Should().BeEquivalentTo(DbInitializer.CartItems.Where(x => x.ClientId == DbInitializer.ClientIdOne).ToListGetDTO());
    }

    [Theory]
    [InlineData(null)]
    [InlineData(default)]
    public async Task GetCart_WithNullOrDefaultClientId_ReturnsNotFound(Guid id)
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/cart/" + id);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var response = await result.Content.ReadAsStringAsync();
        response.Should().NotBeNull();
        response.Should().Be("The client could not be found.");
    }

    [Theory]
    [InlineData("ProductA", 19.99, 2, "thumbnail1.jpg", 0.25)]
    [InlineData("ProductB", 29.95, 1, "thumbnail2.jpg")]
    public async Task AddProductToCart_WithValidClientIdAndData_ReturnsOk(string name, double price, int amount, string thumbnailUrl, double discount = 0)
    {
        // Arrage
        CartItemPostDTO dto = new CartItemPostDTO()
        {
            ProductId = Guid.NewGuid(),
            ThumbnailUrl = thumbnailUrl,
            Name = name,
            Price = price,
            Amount = amount,
            DiscountApplied = discount
        };

        // Act
        var result = await _client.PostAsJsonAsync("/cart/" + DbInitializer.ClientIdOne, dto);

        // Assert
        result.EnsureSuccessStatusCode();

        var response = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        response.Should().NotBeNull();
        response.IsSuccess.Should().Be(true);
        response.Error.Should().BeNullOrEmpty();

        CartItem? item = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            item = await context.CartItems.Where(x => x.ProductId == dto.ProductId && x.ClientId == DbInitializer.ClientIdOne).SingleOrDefaultAsync();
        }

        item.Should().NotBeNull();

        var transformedItem = new CartItem(
            clientId: item.ClientId,
            productId: item.ProductId,
            thumbnailUrl: item.ThumbnailUrl,
            name: item.Name,
            price: item.Price,
            amount: item.Amount
        );

        transformedItem.Should().BeEquivalentTo(
            new CartItem(
                clientId: DbInitializer.ClientIdOne, 
                productId: dto.ProductId,
                thumbnailUrl: dto.ThumbnailUrl,
                name: dto.Name,
                price: dto.Price,
                amount: dto.Amount
            )         
        );
    }

    [Theory]
    [InlineData("11111111-1111-1111-1111-111111111119", null, "ProductB", 29.95, 1)]
    [InlineData("11111111-1111-1111-1111-111111111119", "", "ProductB", 29.95, 1)]
    [InlineData("11111111-1111-1111-1111-111111111119", "thumbnail3.jpg", null, 39.99, 3)]
    [InlineData("11111111-1111-1111-1111-111111111119", "thumbnail3.jpg", "", 39.99, 3)]
    [InlineData("11111111-1111-1111-1111-111111111119", "thumbnail4.jpg", "ProductC", -5.0, 2)]
    [InlineData("11111111-1111-1111-1111-111111111119", "thumbnail5.jpg", "ProductD", 15.00, -1)]
    [InlineData("11111111-1111-1111-1111-111111111119", "thumbnail6.jpg", "ProductE", 25.50, 2, 2.0)]
    [InlineData("11111111-1111-1111-1111-111111111119", "thumbnail6.jpg", "ProductE", 25.50, 2, -1.0)]
    public async Task AddProductToCart_WithInvalidData_ReturnsBadRequest(Guid productId, string thumbnailUrl, string name, double price, int amount, double discount = 0)
    {
        // Arrage
        CartItemPostDTO dto = new CartItemPostDTO()
        {
            ProductId = productId,
            ThumbnailUrl = thumbnailUrl,
            Name = name,
            Price = price,
            Amount = amount,
            DiscountApplied = discount
        };

        // Act
        var result = await _client.PostAsJsonAsync("/cart/" + DbInitializer.ClientIdOne, dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var response = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        response.Should().NotBeNull();
        response.IsSuccess.Should().Be(false);
        response.Error.Should().NotBeNullOrWhiteSpace();

        CartItem? item = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            item = await context.CartItems.Where(x => x.ProductId == dto.ProductId && x.ClientId == DbInitializer.ClientIdOne).SingleOrDefaultAsync();
        }

        item.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(default)]
    public async Task AddProductToCart_WithNullOrDefaultClientId_ReturnsNotFound(Guid clientId)
    {
        // Arrage
        CartItemPostDTO dto = new CartItemPostDTO()
        {
            ProductId = DbInitializer.CartItems[0].ProductId,
            ThumbnailUrl = DbInitializer.CartItems[0].ThumbnailUrl,
            Name = DbInitializer.CartItems[0].Name,
            Price = DbInitializer.CartItems[0].Price,
            Amount = DbInitializer.CartItems[0].Amount
        };

        // Act
        var result = await _client.PostAsJsonAsync("/cart/" + clientId, dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var response = await result.Content.ReadAsStringAsync();
        response.Should().NotBeNull();
        response.Should().Be("The client could not be found.");

        CartItem? item = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            item = await context.CartItems.Where(x => x.ProductId == dto.ProductId && x.ClientId == clientId).SingleOrDefaultAsync();
        }

        item.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(default)]
    public async Task AddProductToCart_WithNullOrDefaultProductId_ReturnsNotFound(Guid productId)
    {
        // Arrage
        CartItemPostDTO dto = new CartItemPostDTO()
        {
            ProductId = productId,
            ThumbnailUrl = DbInitializer.CartItems[0].ThumbnailUrl,
            Name = DbInitializer.CartItems[0].Name,
            Price = DbInitializer.CartItems[0].Price,
            Amount = DbInitializer.CartItems[0].Amount
        };

        // Act
        var result = await _client.PostAsJsonAsync("/cart/" + DbInitializer.ClientIdOne, dto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var response = await result.Content.ReadAsStringAsync();
        response.Should().NotBeNull();
        response.Should().Be("The product could not be found.");

        CartItem? item = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            item = await context.CartItems.Where(x => x.ProductId == dto.ProductId && x.ClientId == DbInitializer.ClientIdOne).SingleOrDefaultAsync();
        }

        item.Should().BeNull();
    }

    [Fact]
    public async Task TransferCartItems_OriginAndDestinyWithData_AddsNonexistentToDestinyAndOriginGetsRemoved()
    {
        // Arrage
        List<CartItem> expectedOutputClientOne = new List<CartItem>(DbInitializer.CartItems.Where(x => x.ClientId == DbInitializer.ClientIdOne));

        expectedOutputClientOne.AddRange(
            DbInitializer.CartItems
                .Where(x => x.ClientId == DbInitializer.ClientIdTwo)
                .Where(x => !expectedOutputClientOne.Select(y => y.ProductId).Contains(x.ProductId))
                .Select(x => new CartItem(DbInitializer.ClientIdOne, x.ProductId, x.ThumbnailUrl, x.Name, x.Price, x.Amount))
        );

        // Act
        var result = await _client.PutAsync("/cart/" + DbInitializer.ClientIdTwo + "/transfer-to/" + DbInitializer.ClientIdOne, null);

        // Assert
        result.EnsureSuccessStatusCode();

        var response = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        response.Should().NotBeNull();
        response.IsSuccess.Should().Be(true);
        response.Error.Should().BeNullOrEmpty();

        List<CartItem> itemsOnClientOne = new();
        List<CartItem> itemsOnClientTwo = new();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            itemsOnClientOne = await context.CartItems.Where(x => x.ClientId == DbInitializer.ClientIdOne).ToListAsync();
            itemsOnClientTwo = await context.CartItems.Where(x => x.ClientId == DbInitializer.ClientIdTwo).ToListAsync();
        }

        itemsOnClientOne.Should().NotBeNull();
        itemsOnClientOne.Select(item=> 
            new CartItem(
                clientId: item.ClientId,
                productId: item.ProductId,
                thumbnailUrl: item.ThumbnailUrl,
                name: item.Name,
                price: item.Price,
                amount: item.Amount
            )
        ).Should().BeEquivalentTo(expectedOutputClientOne);
        itemsOnClientTwo.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(default, default)]
    [InlineData("11111111-1111-1111-1111-111111111111", null)]
    [InlineData("11111111-1111-1111-1111-111111111111", default)]
    [InlineData(null, "11111111-1111-1111-1111-111111111111")]
    [InlineData(default, "11111111-1111-1111-1111-111111111111")]
    public async Task TransferCartItems_WithInvalidOriginOrDestinyGuid_AddsNonexistentToDestinyAndOriginGetsRemoved(Guid from, Guid to)
    {
        // Arrage

        // Act
        var result = await _client.PutAsync("/cart/" + from + "/transfer-to/" + to, null);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var response = await result.Content.ReadAsStringAsync();
        response.Should().NotBeNull();
        response.Should().NotBeNullOrWhiteSpace();
    }
}
