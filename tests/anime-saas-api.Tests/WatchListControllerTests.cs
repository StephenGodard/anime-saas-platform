using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AnimeSaasApi.Dtos;
using anime_saas_api.Tests.Factory;
using Xunit;

namespace anime_saas_api.Tests;

public class WatchListControllerTests : IClassFixture<WatchListItemWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WatchListControllerTests(WatchListItemWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task SetupTestData()
    {
        // Create test user
        await _client.PostAsJsonAsync("/api/User", new {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        });
        
        // Create test anime
        await _client.PostAsJsonAsync("/api/Anime", new {
            Title = "Test Anime",
            Classification = "Shonen",
            MyAnimeListId = 12345,
            Genres = new List<int>(),
            Platforms = new List<int>(),
            Studios = new List<int>()
        });
    }

    [Fact]
    public async Task GetAllWatchListItems_ReturnsOk()
    {
        // Arrange
        await SetupTestData();
        await _client.PostAsJsonAsync("api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "Watching",
            Progression = 12
        });

        // Act
        var response = await _client.GetAsync("api/watchlist-items");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<WatchListItemReadDto>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        var item = items[0];
        Assert.Equal(1, item.UserId);
        Assert.Equal("Test Anime", item.AnimeTitle);
        Assert.Equal(9, item.Score);
        Assert.Equal("Watching", item.Status);
    }

    [Fact]
    public async Task GetByUserId_ReturnsOk()
    {
        // Arrange
        await SetupTestData();
        await _client.PostAsJsonAsync("api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "Completed",
            Progression = 12,
            CreatedAt = DateTime.UtcNow
        });
        
        // Act
        var response = await _client.GetAsync("api/watchlist-items/user/1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<WatchListItemReadDto>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        var item = items[0];
        Assert.Equal(1, item.UserId);
        Assert.Equal("Test Anime", item.AnimeTitle);
        Assert.Equal("Completed", item.Status);
        Assert.Equal(9, item.Score);
        Assert.Equal(12, item.Progression);
        Assert.Equal("Excellent anime!", item.Comment);
    }

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        // Arrange
        await SetupTestData();
        var createResponse = await _client.PostAsJsonAsync("api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "Watching",
            Progression = 12
        });
        createResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _client.GetAsync("api/watchlist-items/1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var item = await response.Content.ReadFromJsonAsync<WatchListItemReadDto>();
        Assert.NotNull(item);
        Assert.Equal(1, item.UserId);
        Assert.Equal("Test Anime", item.AnimeTitle);
        Assert.Equal("Watching", item.Status);
    }

    [Fact]
    public async Task CreateWatchListItem_ReturnsCreated()
    {
        // Arrange
        await SetupTestData();
        var dto = new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "PlanToWatch",
            Progression = 0,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/watchlist-items", dto);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdItem = await response.Content.ReadFromJsonAsync<WatchListItemReadDto>();
        Assert.NotNull(createdItem);
        Assert.Equal(1, createdItem.UserId);
        Assert.Equal(1, createdItem.AnimeId);
        Assert.Equal(9, createdItem.Score);
        Assert.Equal("PlanToWatch", createdItem.Status);
    }

    [Fact]
    public async Task UpdateWatchListItem_ReturnsOk()
    {
        // Arrange
        await SetupTestData();
        await _client.PostAsJsonAsync("api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 8,
            Comment = "Good anime!",
            Status = "Watching",
            Progression = 5
        });

        var updateDto = new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 10,
            Comment = "Masterpiece!",
            Status = "Completed",
            Progression = 24
        };

        // Act
        var response = await _client.PutAsJsonAsync("api/watchlist-items/1", updateDto);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        // Verify the update was successful
        var getResponse = await _client.GetAsync("api/watchlist-items/1");
        getResponse.EnsureSuccessStatusCode();
        var updatedItem = await getResponse.Content.ReadFromJsonAsync<WatchListItemReadDto>();
        Assert.NotNull(updatedItem);
        Assert.Equal(10, updatedItem.Score);
        Assert.Equal("Masterpiece!", updatedItem.Comment);
        Assert.Equal("Completed", updatedItem.Status);
        Assert.Equal(24, updatedItem.Progression);
    }

    [Fact]
    public async Task DeleteWatchListItem_ReturnsNoContent()
    {
        // Arrange
        await SetupTestData();
        await _client.PostAsJsonAsync("api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "Completed",
            Progression = 12
        });
        
        // Act
        var response = await _client.DeleteAsync("api/watchlist-items/1");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify item was deleted
        var getResponse = await _client.GetAsync("api/watchlist-items/1");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateWatchListItem_WithInvalidStatus_ReturnsDefaultStatus()
    {
        // Arrange
        await SetupTestData();
        var dto = new WatchListItemCreateDto
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "InvalidStatus", // Invalid status
            Progression = 12
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/watchlist-items", dto);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdItem = await response.Content.ReadFromJsonAsync<WatchListItemReadDto>();
        Assert.NotNull(createdItem);
        // Should default to PlanToWatch as per controller logic
        Assert.Equal("PlanToWatch", createdItem.Status);
    }
}