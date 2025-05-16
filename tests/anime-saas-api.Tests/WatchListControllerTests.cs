using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AnimeSaasApi.Dtos;
using anime_saas_api.Tests.Factory;
using Xunit;
using Xunit.Abstractions;

namespace anime_saas_api.Tests;

public class WatchListControllerTests : IClassFixture<WatchListItemWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
    { 
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private readonly ITestOutputHelper _output;

    public WatchListControllerTests(WatchListItemWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    private async Task<(int userId, int animeId)> SetupTestData()
    {
        // Créer un utilisateur unique pour ce test
        var username = $"testuser{Guid.NewGuid()}";
        var email = $"test{Guid.NewGuid()}@example.com";
        
        var userResponse = await _client.PostAsJsonAsync("/api/User", new {
            Username = username,
            Email = email,
            Password = "Password123!"
        });
        userResponse.EnsureSuccessStatusCode();
        var userContent = await userResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API User Response: {userContent}");
        var user = JsonSerializer.Deserialize<UserReadDto>(userContent, _jsonOptions);
        
        // Créer un anime unique pour ce test
        var animeTitle = $"Test Anime {Guid.NewGuid()}";
        var animeResponse = await _client.PostAsJsonAsync("/api/Anime", new {
            Title = animeTitle,
            Classification = "Shonen",
            GenreIds = new List<int> { 1 },
            PlatformIds = new List<int>(),
            StudioIds = new List<int> { 1 }
        });
        animeResponse.EnsureSuccessStatusCode();
        var animeContent = await animeResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API Anime Response: {animeContent}");
        
        // Utiliser un typage dynamique pour éviter les problèmes de désérialisation
        using var animeDoc = JsonDocument.Parse(animeContent);
        int animeId = animeDoc.RootElement.GetProperty("id").GetInt32();
        
        return (user?.Id ?? 1, animeId);
    }

    [Fact]
    public async Task GetAllWatchListItems_ReturnsOk()
    {
        // Arrange
        var (userId, animeId) = await SetupTestData();
        
        var itemToCreate = new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "Watching",
            Progression = 12
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/watchlist-items", itemToCreate);
        createResponse.EnsureSuccessStatusCode();
        var createContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API Create Response: {createContent}");

        // Act
        var response = await _client.GetAsync("/api/watchlist-items");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"API GetAll Response: {content}");
        var items = JsonSerializer.Deserialize<List<WatchListItemReadDto>>(content, _jsonOptions);
        Assert.NotNull(items);
        Assert.Contains(items, item => item.UserId == userId && item.AnimeId == animeId);
    }

    [Fact]
    public async Task GetByUserId_ReturnsOk()
    {
        // Arrange
        var (userId, animeId) = await SetupTestData();
        
        var commentText = $"Excellent anime! {Guid.NewGuid()}"; 
        
        var createResponse = await _client.PostAsJsonAsync("/api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 9,
            Comment = commentText,
            Status = "Completed",
            Progression = 12
        });
        createResponse.EnsureSuccessStatusCode();
        var createContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API Create Response: {createContent}");
        
        // Act
        var response = await _client.GetAsync($"/api/watchlist-items/user/{userId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"API GetByUserId Response: {content}");
        var items = JsonSerializer.Deserialize<List<WatchListItemReadDto>>(content, _jsonOptions);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        
        var item = items.Find(i => i.UserId == userId && i.Comment == commentText);
        Assert.NotNull(item);
        Assert.Equal("Completed", item.Status);
        Assert.Equal(9, item.Score);
        Assert.Equal(12, item.Progression);
    }

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        // Arrange
        var (userId, animeId) = await SetupTestData();
        
        var createResponse = await _client.PostAsJsonAsync("/api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "Watching",
            Progression = 12
        });
        createResponse.EnsureSuccessStatusCode();
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API Create Response: {createContent}");
        
        // Extraire l'ID directement du JSON
        using var jsonDoc = JsonDocument.Parse(createContent);
        int itemId = jsonDoc.RootElement.GetProperty("id").GetInt32();
        
        // Act
        var response = await _client.GetAsync($"/api/watchlist-items/{itemId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"API GetById Response: {content}");
        var item = JsonSerializer.Deserialize<WatchListItemReadDto>(content, _jsonOptions);
        Assert.NotNull(item);
        Assert.Equal(userId, item.UserId);
        Assert.Equal("Watching", item.Status);
    }

    [Fact]
    public async Task CreateWatchListItem_ReturnsCreated()
    {
        // Arrange
        var (userId, animeId) = await SetupTestData();
        
        var dto = new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "PlanToWatch",
            Progression = 0
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/watchlist-items", dto);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"API Create Response: {content}");

        // Extraire l'ID directement du JSON pour éviter les problèmes de désérialisation
        using var jsonDoc = JsonDocument.Parse(content);
        int itemId = jsonDoc.RootElement.GetProperty("id").GetInt32();
        Assert.NotEqual(0, itemId);
        
        // Vérifier les autres propriétés
        var createdItem = JsonSerializer.Deserialize<WatchListItemReadDto>(content, _jsonOptions);
        Assert.NotNull(createdItem);
        Assert.Equal(userId, createdItem.UserId);
        Assert.Equal(animeId, createdItem.AnimeId);
        Assert.Equal(9, createdItem.Score);
        Assert.Equal("PlanToWatch", createdItem.Status);
        Assert.NotEqual(default, createdItem.DateUpdate);
    }

    [Fact]
    public async Task UpdateWatchListItem_ReturnsOk()
    {
        // Arrange
        var (userId, animeId) = await SetupTestData();
        
        // Créer un élément
        var createResponse = await _client.PostAsJsonAsync("/api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 8,
            Comment = "Good anime!",
            Status = "Watching",
            Progression = 5
        });
        createResponse.EnsureSuccessStatusCode();
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API Create Response: {createContent}");
        
        // Extraire l'ID directement du JSON
        using var jsonDoc = JsonDocument.Parse(createContent);
        int itemId = jsonDoc.RootElement.GetProperty("id").GetInt32();
        DateTime originalDateUpdate = DateTime.Parse(jsonDoc.RootElement.GetProperty("dateUpdate").GetString());

        var updateDto = new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 10,
            Comment = "Masterpiece!",
            Status = "Completed",
            Progression = 24
        };

        // Attendre un peu pour être sûr que DateUpdate sera différente
        await Task.Delay(10);
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/watchlist-items/{itemId}", updateDto);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        // Vérifier que la mise à jour a fonctionné
        var getResponse = await _client.GetAsync($"/api/watchlist-items/{itemId}");
        getResponse.EnsureSuccessStatusCode();
        
        var content = await getResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API Get Updated Response: {content}");
        
        using var updatedDoc = JsonDocument.Parse(content);
        DateTime newDateUpdate = DateTime.Parse(updatedDoc.RootElement.GetProperty("dateUpdate").GetString());
        Assert.True(newDateUpdate > originalDateUpdate);
        
        var updatedItem = JsonSerializer.Deserialize<WatchListItemReadDto>(content, _jsonOptions);
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
        var (userId, animeId) = await SetupTestData();
        
        var createResponse = await _client.PostAsJsonAsync("/api/watchlist-items", new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "Completed",
            Progression = 12
        });
        createResponse.EnsureSuccessStatusCode();
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"API Create Response: {createContent}");
        
        // Extraire l'ID directement du JSON
        using var jsonDoc = JsonDocument.Parse(createContent);
        int itemId = jsonDoc.RootElement.GetProperty("id").GetInt32();
        
        // Attendre un peu pour éviter les problèmes de concurrence
        await Task.Delay(100);
        
        // Act
        var response = await _client.DeleteAsync($"/api/watchlist-items/{itemId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Vérifier que l'élément a été supprimé
        var getResponse = await _client.GetAsync($"/api/watchlist-items/{itemId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateWatchListItem_WithInvalidStatus_ReturnsDefaultStatus()
    {
        // Arrange
        var (userId, animeId) = await SetupTestData();
        
        var dto = new WatchListItemCreateDto
        {
            UserId = userId,
            AnimeId = animeId,
            Score = 9,
            Comment = "Excellent anime!",
            Status = "InvalidStatus", // Statut invalide
            Progression = 12
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/watchlist-items", dto);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"API Create Response with Invalid Status: {content}");
        
        using var jsonDoc = JsonDocument.Parse(content);
        string status = jsonDoc.RootElement.GetProperty("status").GetString();
        Assert.Equal("PlanToWatch", status);
    }
}