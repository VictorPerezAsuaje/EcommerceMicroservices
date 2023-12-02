using Azure.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Catalog.Application;
using Services.Catalog.Application.Products;
using Services.Catalog.Application.Tags;
using Services.Catalog.Domain;
using Services.Catalog.Infrastructure;
using Services.Catalog.Tests;
using Services.Catalog.Tests.Utilities;
using System.Net;
using System.Net.Http.Json;

namespace Services.Catalog.Tests.Integration;

[Collection("Catalog")]
public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        factory.CleanupDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsProductList()
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/products");

        // Assert
        result.EnsureSuccessStatusCode();
        
        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<List<ProductGetDTO>>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var products = tokenResult.Value;
        products.Should().OnlyHaveUniqueItems();
        products.Should().HaveCount(DbInitializer.Products.Count);
        products.Should().BeEquivalentTo(DbInitializer.Products.ToListGetDTO());
    }

    [Fact]
    public async Task GetById_WithExistentId_ReturnsProductRequested()
    {
        // Arrage
        Guid id = DbInitializer.Products[0].Id;

        // Act
        var result = await _client.GetAsync("/products/" + id);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<ProductGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var product = tokenResult.Value;
        product.Should().BeEquivalentTo(DbInitializer.Products[0].ToGetDTO());
    }

    [Fact]
    public async Task GetByName_WithUnexistentId_ReturnsBadRequest()
    {
        // Arrage
        Guid id = Guid.NewGuid();

        // Act
        var result = await _client.GetAsync("/products/" + id);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<ProductGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();
        tokenResult.Value.Should().BeNull();
    }

    [Theory]
    [InlineData("ProductA", 29.99, "Electronics", "Tech", true, "A cool gadget", "High-tech details")]
    [InlineData("ProductB", 15.0, null, null, null, null, null)] // Valid with all nullable props
    [InlineData("ProductC", 25.0, Category.Default, null, null, null, null)] // Valid with null tags
    [InlineData("ProductD", 39.99, Category.Default, Tag.Default, null, null, null)] // Valid with one null tag
    [InlineData("ProductE", 59.95, Category.Default, Tag.Default, true, "Awesome toy", null)] // Valid with null details
    public async Task Create_WithValidData_ReturnsOk(string name, double price, string defaultCategory, string defaultTag, bool secondTag, string description, string details)
    {
        // Arrage
        List<string>? tags = null;

        if (!string.IsNullOrWhiteSpace(defaultTag))
        {
            tags = new List<string>() { defaultTag };

            if (secondTag)
                tags.Add(DbInitializer.Tags[1].Name);
        }


        ProductPostDTO request = new ProductPostDTO()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Category = defaultCategory,
            Tags = tags,
            Description = description,
            Details = details
        };

        // Act
        var result = await _client.PostAsJsonAsync("/products", request);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<Guid>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrWhiteSpace();
        tokenResult.Value.Should().Be(request.Id);

        Product? product = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            product = await context.Products.Where(x => x.Id == request.Id).SingleOrDefaultAsync();
        }

        product.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithoutCategory_ReturnsOkAndCategoryIsSetToDefault()
    {
        // Arrage
        ProductPostDTO request = new ProductPostDTO()
        {
            Id = Guid.NewGuid(),
            Name = "ProductF",
            Price = 105.0
        };

        // Act
        var result = await _client.PostAsJsonAsync("/products", request);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<Guid>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrWhiteSpace();
        tokenResult.Value.Should().Be(request.Id);

        Product? product = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            product = await context.Products.Where(x => x.Id == request.Id).SingleOrDefaultAsync();
        }

        product.Should().NotBeNull();
        product.CategoryName.Should().Be(Category.Default);
    }

    [Theory]
    [InlineData("", 29.99, Category.Default, "A cool gadget", "High-tech details")]
    [InlineData(null, 29.99, Category.Default, "A cool gadget", "High-tech details")]
    [InlineData("ProductA", null, Category.Default, "A cool gadget", "High-tech details")]
    [InlineData("ProductA", -1.05, Category.Default, "A cool gadget", "High-tech details")]
    [InlineData("ProductA", 29.99, Category.Default, DbInitializer.StringWithMoreThan2000Characters, "High-tech details")]
    [InlineData("ProductA", 29.99, Category.Default, "A cool gadget", DbInitializer.StringWithMoreThan2000Characters)]
    [InlineData("ProductA", 29.99, "ThisCategoryDoesNotExist", "A cool gadget", DbInitializer.StringWithMoreThan2000Characters)]
    public async Task Create_WithInvalidData_ReturnsBadRequest(string name, double price, string category, string description, string details)
    {
        // Arrage
        ProductPostDTO request = new ProductPostDTO()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Category = category,
            Description = description,
            Details = details
        };

        // Act
        var result = await _client.PostAsJsonAsync("/products", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<Guid>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();

        Product? product = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            product = await context.Products.Where(x => x.Id == request.Id).SingleOrDefaultAsync();
        }

        product.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsOkAndDeletesProduct()
    {
        // Arrage
        Guid idToDelete = DbInitializer.Products[0].Id;

        Product? product = null;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            product = await context.Products.Where(x => x.Id == idToDelete).SingleOrDefaultAsync();
        }

        // Act
        var result = await _client.DeleteAsync("/products/" + idToDelete);

        // Assert

        product.Should().NotBeNull();
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrWhiteSpace();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            product = await context.Products.Where(x => x.Id == idToDelete).SingleOrDefaultAsync();
        }

        product.Should().BeNull();
    }

    [Fact]
    public async Task Delete_UnexistingId_ReturnsNotFound()
    {
        // Arrage
        Guid idToDelete = Guid.NewGuid();

        Product? product = null;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            product = await context.Products.Where(x => x.Id == idToDelete).SingleOrDefaultAsync();
        }

        // Act
        var result = await _client.DeleteAsync("/products/" + idToDelete);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        product.Should().BeNull();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            product = await context.Products.Where(x => x.Id == idToDelete).SingleOrDefaultAsync();
        }

        product.Should().BeNull();
    }
}
