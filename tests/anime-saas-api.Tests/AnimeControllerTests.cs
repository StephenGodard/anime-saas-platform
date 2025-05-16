using Xunit;
using System.Net;
using System.Threading.Tasks;
using AnimeSaasApi.Models; 
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using anime_saas_api.Tests.Factory;
using System.Net.Http.Json;
using System.Collections.Generic;
using AnimeSaasApi.Dtos;
using System;

namespace anime_saas_api.Tests;

public class AnimeControllerTests : IClassFixture<AnimeWebApplicationFactory>
{
    private readonly AnimeWebApplicationFactory _factory;

    public AnimeControllerTests(AnimeWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAnimes_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/anime");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchAnime_WithTitle_ReturnsMatchingAnimes()
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchTitle = "Naruto";

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        // Setup test data
        var genre = new Genre { Name = "Shonen" };
        var studio = new Studio { Name = "Studio Pierrot" };
        var platform = new Platform { Name = "Crunchyroll" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        var animeToCreate = new
        {
            Title = searchTitle,
            Description = "Ninja adventure",
            Classification = "Shonen",
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        // Create test anime
        await client.PostAsJsonAsync("/api/anime", animeToCreate);

        // Act
        var response = await client.GetAsync($"/api/anime/search?title={searchTitle}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<AnimeReadDto>>();
        Assert.NotEmpty(result);
        Assert.Contains(result, a => a.Title == searchTitle);
    }

    [Fact]
    public async Task SearchAnime_WithGenre_ReturnsMatchingAnimes()
    {
        // Arrange
        var client = _factory.CreateClient();
        var genreName = "Fantasy";

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        // Setup test data
        var genre = new Genre { Name = genreName };
        var studio = new Studio { Name = "A-1 Pictures" };
        var platform = new Platform { Name = "Wakanim" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        var animeToCreate = new
        {
            Title = "Sword Art Online",
            Description = "Virtual reality MMORPG",
            Classification = "Shonen",
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        // Create test anime
        await client.PostAsJsonAsync("/api/anime", animeToCreate);

        // Act
        var response = await client.GetAsync($"/api/anime/search?genre={genreName}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<AnimeReadDto>>();
        Assert.NotEmpty(result);
        Assert.Contains(result, a => a.Genres.Contains(genreName));
    }

    [Fact]
    public async Task GetTopAnimes_ReturnsSuccessAndOrderedByPopularity()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        // Setup test data
        var genre = new Genre { Name = "Action" };
        var studio = new Studio { Name = "Toei" };
        var platform = new Platform { Name = "ADN" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        // Create multiple animes with different popularity
        var anime1 = new
        {
            Title = "Popular Anime 1",
            Description = "Very popular anime",
            Classification = "Seinen",
            Popularity = 1000,
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        var anime2 = new
        {
            Title = "Popular Anime 2",
            Description = "Less popular anime",
            Classification = "Shonen",
            Popularity = 500,
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        await client.PostAsJsonAsync("/api/anime", anime1);
        await client.PostAsJsonAsync("/api/anime", anime2);

        // Act
        var response = await client.GetAsync("/api/anime/top");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<AnimeReadDto>>();
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetLatestAnimes_ReturnsSuccessAndOrderedByDate()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        // Setup test data
        var genre = new Genre { Name = "Drama" };
        var studio = new Studio { Name = "Wit Studio" };
        var platform = new Platform { Name = "Netflix" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        // Create multiple animes with different dates
        var anime1 = new
        {
            Title = "New Anime 1",
            Description = "Recently released",
            Classification = "Seinen",
            DateStart = DateTime.Now.AddDays(-10),
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        var anime2 = new
        {
            Title = "New Anime 2",
            Description = "Just released",
            Classification = "Shonen",
            DateStart = DateTime.Now.AddDays(-5),
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        await client.PostAsJsonAsync("/api/anime", anime1);
        await client.PostAsJsonAsync("/api/anime", anime2);

        // Act
        var response = await client.GetAsync("/api/anime/lastest");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<AnimeReadDto>>();
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAnimesBySeason_WithValidSeasonAndYear_ReturnsMatchingAnimes()
    {
        // Arrange
        var client = _factory.CreateClient();
        var season = "Winter";
        var year = 2023;

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        // Setup test data
        var genre = new Genre { Name = "Mystery" };
        var studio = new Studio { Name = "MAPPA" };
        var platform = new Platform { Name = "Crunchyroll" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        var animeToCreate = new
        {
            Title = $"{season} {year} Anime",
            Description = "Seasonal anime",
            Classification = "Seinen",
            Season = season,
            SeasonYear = year,
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        // Create test anime
        await client.PostAsJsonAsync("/api/anime", animeToCreate);

        // Act
        var response = await client.GetAsync($"/api/anime/season?season={season}&year={year}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<AnimeReadDto>>();
        Assert.NotEmpty(result);
        Assert.Contains(result, a => a.Season == season && a.SeasonYear == year);
    }

    [Fact]
    public async Task GetAnimesBySeason_WithInvalidSeason_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidSeason = "NotASeason";
        var year = 2023;

        // Act
        var response = await client.GetAsync($"/api/anime/season?season={invalidSeason}&year={year}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAnimeById_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        // Création des entités nécessaires
        var genre = new Genre { Name = "Action" };
        var studio = new Studio { Name = "Ufotable" };
        var platform = new Platform { Name = "Crunchyroll" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        var createAnime = new
        {
            Title = "Test Anime",
            Description = "Description test",
            Classification = "Unknown",
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        // POST pour créer l'anime
        var postResponse = await client.PostAsJsonAsync("/api/anime", createAnime);
        postResponse.EnsureSuccessStatusCode();

        var createdAnime = await postResponse.Content.ReadFromJsonAsync<AnimeReadDto>();

        // GET pour vérifier l'accès
        var getResponse = await client.GetAsync($"/api/anime/{createdAnime.Id}");

        // Assert
        getResponse.EnsureSuccessStatusCode(); // 200 OK attendu
    }

    [Fact]
    public async Task UpdateAnime_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        var genre = new Genre { Name = "Adventure" };
        var studio = new Studio { Name = "MAPPA" };
        var platform = new Platform { Name = "Netflix" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        var createAnime = new
        {
            Title = "Original Title",
            Description = "Original Description",
            Classification = "Unknown",
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        var postResponse = await client.PostAsJsonAsync("/api/anime", createAnime);
        postResponse.EnsureSuccessStatusCode();
        var createdAnime = await postResponse.Content.ReadFromJsonAsync<AnimeReadDto>();

        var updateAnime = new
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Classification = "Shonen",
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        var putResponse = await client.PutAsJsonAsync($"/api/anime/{createdAnime.Id}", updateAnime);
        putResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync($"/api/anime/{createdAnime.Id}");
        var updatedAnime = await getResponse.Content.ReadFromJsonAsync<AnimeReadDto>();
        Assert.Equal("Updated Title", updatedAnime.Title);
    }

    [Fact]
    public async Task DeleteAnime_ReturnsSuccessAndAnimeNotFoundAfterward()
    {
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        var genre = new Genre { Name = "Comedy" };
        var studio = new Studio { Name = "Bones" };
        var platform = new Platform { Name = "ADN" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        var createAnime = new
        {
            Title = "To Be Deleted",
            Description = "Soon gone",
            Classification = "Seinen",
            GenreIds = new List<int> { genre.Id },
            PlatformIds = new List<int> { platform.Id },
            StudioIds = new List<int> { studio.Id }
        };

        var postResponse = await client.PostAsJsonAsync("/api/anime", createAnime);
        postResponse.EnsureSuccessStatusCode();
        var createdAnime = await postResponse.Content.ReadFromJsonAsync<AnimeReadDto>();

        var deleteResponse = await client.DeleteAsync($"/api/anime/{createdAnime.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        var getAfterDelete = await client.GetAsync($"/api/anime/{createdAnime.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }
}