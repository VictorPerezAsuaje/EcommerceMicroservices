using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Services.Catalog.Application;
using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Products;
using Services.Catalog.Application.Tags;
using Services.Catalog.Infrastructure;
using Services.Catalog.Tests;
using Services.Catalog.Tests.Utilities;
using System.Net;
using System.Net.Http.Json;

namespace Services.Catalog.Tests.Integration;

[Collection("Catalog")]
public class TagControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TagControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        factory.CleanupDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsTagList()
    {
        // Arrage

        // Act
        var result = await _client.GetAsync("/tags");

        // Assert
        result.EnsureSuccessStatusCode();
        
        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<List<TagGetDTO>>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var tags = tokenResult.Value;
        tags.Should().OnlyHaveUniqueItems();
        tags.Should().HaveCount(DbInitializer.Tags.Count);
        tags.Should().BeEquivalentTo(DbInitializer.Tags.ToListGetDTO());
    }

    [Fact]
    public async Task GetById_WithExistentName_ReturnsTagRequested()
    {
        // Arrage
        string name = DbInitializer.Tags[1].Name;

        // Act
        var result = await _client.GetAsync("/tags/" + name);

        // Assert
        result.EnsureSuccessStatusCode();

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<TagGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(true);
        tokenResult.Error.Should().BeNullOrEmpty();

        var tag = tokenResult.Value;
        tag.Should().BeEquivalentTo(DbInitializer.Tags[1].ToGetDTO());
    }

    [Fact]
    public async Task GetByName_WithUnexistentName_ReturnsBadRequest()
    {
        // Arrage
        string name = "IDontExist";

        // Act
        var result = await _client.GetAsync("/tags/" + name);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var tokenResult = await result.Content.ReadFromJsonAsync<ResponseDTO<TagGetDTO>>();
        tokenResult.Should().NotBeNull();
        tokenResult.IsSuccess.Should().Be(false);
        tokenResult.Error.Should().NotBeNullOrWhiteSpace();
        tokenResult.Value.Should().BeNull();
    }
}
