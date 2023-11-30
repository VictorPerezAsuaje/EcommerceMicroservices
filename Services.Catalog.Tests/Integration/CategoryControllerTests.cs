using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Services.Catalog.Application;
using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Products;
using Services.Catalog.Application.Tags;
using Services.Catalog.Tests.Utilities;
using System.Net;
using System.Net.Http.Json;

namespace Services.Catalog.Tests.Integration;

[Collection("Catalog")]
public class CategoryControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoryControllerTests(CustomWebApplicationFactory factory)
    {
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
}
