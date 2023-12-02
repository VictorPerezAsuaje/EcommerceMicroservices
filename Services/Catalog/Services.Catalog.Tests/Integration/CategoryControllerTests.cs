using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Catalog.Application;
using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Products;
using Services.Catalog.Application.Tags;
using Services.Catalog.Domain;
using Services.Catalog.Infrastructure;
using Services.Catalog.Tests.Utilities;
using System.Net;
using System.Net.Http.Json;

namespace Services.Catalog.Tests.Integration;

[Collection("Catalog")]
public class CategoryControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CategoryControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        factory.CleanupDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsCategoryList()
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/categories");

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<List<CategoryGetDTO>>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var categories = tokenResult.Value;
        categories.Should().OnlyHaveUniqueItems();
        categories.Should().HaveCount(DbInitializer.Categories.Count);
        categories.Should().BeEquivalentTo(DbInitializer.Categories.ToListGetDTO());
    }

    [Fact]
    public async Task GetByName_WithExistentName_ReturnsCategoryRequested()
    {
        // Arrage
        string name = DbInitializer.Categories[1].Name;

        // Act
        var result = await _client.GetAsync("/categories/" + name);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<CategoryGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var category = tokenResult.Value;
        category.Should().BeEquivalentTo(DbInitializer.Categories[1].ToGetDTO());
    }

    [Fact]
    public async Task GetByName_WithUnexistentName_ReturnsBadRequest()
    {
        // Arrage
        string name = "IDontExist";

        // Act
        var result = await _client.GetAsync("/categories/" + name);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<CategoryGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();
        tokenResult.Value.Should().BeNull();
    }

    [Fact]
    public async Task Create_WithNonExistentName_ReturnsOk()
    {
        // Arrage
        CategoryPostDTO request = new CategoryPostDTO() { Name = "CategoryA" };

        // Act
        var result = await _client.PostAsJsonAsync("/categories", request);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrWhiteSpace();

        Category? category = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            category = await context.Categories.Where(x => x.Name == request.Name).SingleOrDefaultAsync();
        }

        category.Should().NotBeNull();
        category.Should().BeEquivalentTo(new Category(request.Name));
    }

    [Fact]
    public async Task Create_WithExistentName_ReturnsBadRequest()
    {
        // Arrage
        CategoryPostDTO request = new CategoryPostDTO() { Name = Category.Default };

        // Act
        var result = await _client.PostAsJsonAsync("/categories", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();

        Category? category = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            category = await context.Categories.Where(x => x.Name == request.Name).SingleOrDefaultAsync();
        }

        category.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Create_WithoutName_ReturnsBadRequest(string name)
    {
        // Arrage
        CategoryPostDTO request = new CategoryPostDTO() { Name = name };

        // Act
        var result = await _client.PostAsJsonAsync("/categories", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();

        Category? category = null;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            category = await context.Categories.Where(x => x.Name == request.Name).SingleOrDefaultAsync();
        }

        category.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ExistingName_ReturnsOkAndDeletesCategory()
    {
        // Arrage
        string name = DbInitializer.Categories[0].Name;

        Category? category = null;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            category = await context.Categories.Where(x => x.Name == name).SingleOrDefaultAsync();
        }

        // Act
        var result = await _client.DeleteAsync("/categories/" + name);

        // Assert

        category.Should().NotBeNull();
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrWhiteSpace();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            category = await context.Categories.Where(x => x.Name == name).SingleOrDefaultAsync();
        }

        category.Should().BeNull();
    }

    [Fact]
    public async Task Delete_UnexistingName_ReturnsNotFound()
    {
        // Arrage
        string name = "ThisCategoriesDoesNotExist";

        Category? category = null;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            category = await context.Categories.Where(x => x.Name == name).SingleOrDefaultAsync();
        }

        // Act
        var result = await _client.DeleteAsync("/categories/" + name);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        category.Should().BeNull();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            category = await context.Categories.Where(x => x.Name == name).SingleOrDefaultAsync();
        }

        category.Should().BeNull();
    }
}
