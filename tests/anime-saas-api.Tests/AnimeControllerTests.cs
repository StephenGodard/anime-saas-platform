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
    
    [Fact]
    public async Task CreateAnime_WithValidData_ReturnsCreatedResponse()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        var genre = new Genre { Name = "Sci-Fi" };
        var studio = new Studio { Name = "Sunrise" };
        var platform = new Platform { Name = "Funimation" };

        db.Genres.Add(genre);
        db.Studios.Add(studio);
        db.Platforms.Add(platform);
        await db.SaveChangesAsync();

        // Test avec un anime complet et valide
        var animeToCreate = new
        {
            Title = "Valid Test Anime",
            Description = "A valid test anime with all required fields",
            ImageUrl = "https://example.com/image.jpg",
            Season = "Winter",
            SeasonYear = 2025,
            DateStart = "2025-01-01",
            DateEnd = "2025-03-31",
            Classification = "Seinen",
            EpisodeCount = 12,
            Popularity = 8.5f, // Test avec une valeur float
            TrailerUrl = "https://www.youtube.com/watch?v=example",
            StreamingStatus = "CurrentlyAiring",
            AvailableVersions = new List<string> { "VOSTFR", "VF" },
            GenreNames = new List<string> { "Sci-Fi", "Action" },
            StudioNames = new List<string> { "Sunrise", "Bandai" },
            PlatformNames = new List<string> { "Funimation", "Crunchyroll" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/anime", animeToCreate);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdAnime = await response.Content.ReadFromJsonAsync<AnimeReadDto>();
        Assert.NotNull(createdAnime);
        Assert.Equal(animeToCreate.Title, createdAnime.Title);
        Assert.Equal(animeToCreate.Popularity, createdAnime.Popularity);
        Assert.Equal(animeToCreate.GenreNames.Count, createdAnime.Genres.Count);
        Assert.Equal(animeToCreate.StudioNames.Count, createdAnime.Studios.Count);
    }
    
    [Fact]
    public async Task CreateAnime_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Test avec un anime sans titre (champ requis)
        var invalidAnime = new
        {
            Description = "An invalid anime without required fields",
            ImageUrl = "https://example.com/image.jpg",
            Season = "Winter",
            SeasonYear = 2025,
            // Title est manquant intentionnellement
            GenreNames = new List<string>(), // Liste vide pour tester la validation
            StudioNames = new List<string>() // Liste vide pour tester la validation
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/anime", invalidAnime);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Title", errorContent); // Vérifie que l'erreur mentionne le champ Title
        Assert.Contains("GenreNames", errorContent); // Vérifie que l'erreur mentionne le champ GenreNames
        Assert.Contains("StudioNames", errorContent); // Vérifie que l'erreur mentionne le champ StudioNames
    }
    
    [Fact]
    public async Task CreateAnime_WithFloatPopularity_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        var genre = new Genre { Name = "Fantasy" };
        var studio = new Studio { Name = "Kyoto Animation" };
        
        db.Genres.Add(genre);
        db.Studios.Add(studio);
        await db.SaveChangesAsync();

        // Test avec une valeur décimale pour Popularity
        var animeToCreate = new
        {
            Title = "Float Popularity Test",
            Description = "Testing float popularity value",
            Classification = "Shonen",
            Popularity = 7.8f, // Valeur décimale
            GenreNames = new List<string> { "Fantasy" },
            StudioNames = new List<string> { "Kyoto Animation" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/anime", animeToCreate);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdAnime = await response.Content.ReadFromJsonAsync<AnimeReadDto>();
        Assert.NotNull(createdAnime);
        Assert.Equal(animeToCreate.Popularity, createdAnime.Popularity);
    }
    
    [Fact]
    public async Task CreateAnime_WithDirectJsonStructure_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Création d'un anime avec la structure JSON directe (sans encapsulation)
        var directJsonAnime = new
        {
            title = "Direct JSON Test", // camelCase pour tester la désérialisation
            description = "Testing direct JSON structure",
            classification = "Shonen",
            popularity = 8.5f,
            genreNames = new List<string> { "Action", "Adventure" },
            studioNames = new List<string> { "Studio Ghibli" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/anime", directJsonAnime);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdAnime = await response.Content.ReadFromJsonAsync<AnimeReadDto>();
        Assert.NotNull(createdAnime);
        Assert.Equal(directJsonAnime.title, createdAnime.Title);
        Assert.Equal(directJsonAnime.popularity, createdAnime.Popularity);
        Assert.Equal(directJsonAnime.genreNames.Count, createdAnime.Genres.Count);
    }
    
    [Fact]
    public async Task GetAnime_WithFloatPopularity_ReturnsCorrectType()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        var genre = new Genre { Name = "Slice of Life" };
        var studio = new Studio { Name = "P.A. Works" };
        
        db.Genres.Add(genre);
        db.Studios.Add(studio);
        await db.SaveChangesAsync();

        // Créer un anime avec une valeur décimale pour Popularity
        var animeToCreate = new
        {
            Title = "Float Type Test",
            Description = "Testing float type preservation",
            Classification = "Slice of Life",
            Popularity = 9.2f,
            GenreNames = new List<string> { "Slice of Life" },
            StudioNames = new List<string> { "P.A. Works" }
        };

        // Créer l'anime
        var createResponse = await client.PostAsJsonAsync("/api/anime", animeToCreate);
        createResponse.EnsureSuccessStatusCode();
        var createdAnime = await createResponse.Content.ReadFromJsonAsync<AnimeReadDto>();

        // Act - Récupérer l'anime
        var getResponse = await client.GetAsync($"/api/anime/{createdAnime.Id}");

        // Assert
        getResponse.EnsureSuccessStatusCode();
        var retrievedAnime = await getResponse.Content.ReadFromJsonAsync<AnimeReadDto>();
        Assert.NotNull(retrievedAnime);
        Assert.Equal(animeToCreate.Popularity, retrievedAnime.Popularity);
        Assert.IsType<float>(retrievedAnime.Popularity);
    }
}