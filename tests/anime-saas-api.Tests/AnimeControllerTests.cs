using Xunit;
using System.Net;
using System.Threading.Tasks;
using AnimeSaasApi.Models; 
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using anime_saas_api.Tests.Factory;
using System.Net.Http.Json;

using AnimeSaasApi.Dtos;

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
                MyAnimeListId = 12345,
                GenreIds = new System.Collections.Generic.List<int> { genre.Id },
                PlatformIds = new System.Collections.Generic.List<int> { platform.Id },
                StudioIds = new System.Collections.Generic.List<int> { studio.Id }
            };

            // POST pour créer l'anime
            var postResponse = await client.PostAsJsonAsync("/api/anime", createAnime);
            var content = await postResponse.Content.ReadAsStringAsync();
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
                MyAnimeListId = 11111,
                GenreIds = new System.Collections.Generic.List<int> { genre.Id },
                PlatformIds = new System.Collections.Generic.List<int> { platform.Id },
                StudioIds = new System.Collections.Generic.List<int> { studio.Id }
            };

            var postResponse = await client.PostAsJsonAsync("/api/anime", createAnime);
            postResponse.EnsureSuccessStatusCode();
            var createdAnime = await postResponse.Content.ReadFromJsonAsync<AnimeReadDto>();

            var updateAnime = new
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Classification = "Shonen",
                MyAnimeListId = 11111,
                GenreIds = new System.Collections.Generic.List<int> { genre.Id },
                PlatformIds = new System.Collections.Generic.List<int> { platform.Id },
                StudioIds = new System.Collections.Generic.List<int> { studio.Id }
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
                MyAnimeListId = 99999,
                GenreIds = new System.Collections.Generic.List<int> { genre.Id },
                PlatformIds = new System.Collections.Generic.List<int> { platform.Id },
                StudioIds = new System.Collections.Generic.List<int> { studio.Id }
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