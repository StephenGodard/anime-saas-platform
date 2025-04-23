using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AnimeSaasApi.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using anime_saas_api.Tests.Factory;
using System.Text.Json;

namespace anime_saas_api.Tests;

public class WatchListControllerTests : IClassFixture<WatchListItemWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WatchListControllerTests(WatchListItemWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByUserId_ReturnsOk()
    {
        // Insert a User and Anime before inserting WatchListItem
        await _client.PostAsJsonAsync("/api/User", new {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        });
        await _client.PostAsJsonAsync("/api/Anime", new {
            Title = "Test Anime",
            Classification = "Shonen",
            MyAnimeListId = 12345,
            Genres = new List<int>(),
            Platforms = new List<int>(),
            Studios = new List<int>()
        });
        // Insert a WatchListItem before testing GET
        await _client.PostAsJsonAsync("api/watchlist-items", new
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime !",
            Status = "Completed",
            Progression = 12
        });
        var response = await _client.GetAsync("api/watchlist-items/user/1");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateWatchListItem_ReturnsCreated()
    {
        // Insert a User and Anime before creating WatchListItem
        await _client.PostAsJsonAsync("/api/User", new {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        });
        await _client.PostAsJsonAsync("/api/Anime", new {
            Title = "Test Anime",
            Classification = "Shonen",
            MyAnimeListId = 12345,
            Genres = new List<int>(),
            Platforms = new List<int>(),
            Studios = new List<int>()
        });
        var dto = new
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime !",
            Status = "Completed",
            Progression = 12
        };

        var response = await _client.PostAsJsonAsync("api/watchlist-items", dto);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateWatchListItem_ReturnsOk()
    {
        // Insert a User and Anime before inserting WatchListItem
        await _client.PostAsJsonAsync("/api/User", new {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        });
        await _client.PostAsJsonAsync("/api/Anime", new {
            Title = "Test Anime",
            Classification = "Shonen",
            MyAnimeListId = 12345,
            Genres = new List<int>(),
            Platforms = new List<int>(),
            Studios = new List<int>()
        });
        // Insert a WatchListItem before testing PUT
        await _client.PostAsJsonAsync("api/watchlist-items", new
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime !",
            Status = "Completed",
            Progression = 12
        });

        var updateDto = new
        {
            UserId = 1,
            AnimeId = 1,
            Score = 10,
            Comment = "Encore mieux !",
            Status = "Completed",
            Progression = 24
        };

        var response = await _client.PutAsJsonAsync("api/watchlist-items/1", updateDto);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteWatchListItem_ReturnsNoContent()
    {
        // Insert a User and Anime before inserting WatchListItem
        await _client.PostAsJsonAsync("/api/User", new {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        });
        await _client.PostAsJsonAsync("/api/Anime", new {
            Title = "Test Anime",
            Classification = "Shonen",
            MyAnimeListId = 12345,
            Genres = new List<int>(),
            Platforms = new List<int>(),
            Studios = new List<int>()
        });
        // Insert a WatchListItem before testing DELETE
        await _client.PostAsJsonAsync("api/watchlist-items", new
        {
            UserId = 1,
            AnimeId = 1,
            Score = 9,
            Comment = "Excellent anime !",
            Status = "Completed",
            Progression = 12
        });
        var response = await _client.DeleteAsync("api/watchlist-items/1");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}
