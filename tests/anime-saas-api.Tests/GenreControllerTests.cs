using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AnimeSaasApi.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using anime_saas_api.Tests.Factory;

namespace anime_saas_api.Tests;

public class GenreControllerTests : IClassFixture<GenreWebApplicationFactory>
{
    private readonly GenreWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GenreControllerTests(GenreWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateGenre_ReturnsCreated()
    {
        var genreDto = new GenreCreateDto { Name = "Action" };

        var response = await _client.PostAsJsonAsync("/api/genre", genreDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdGenre = await response.Content.ReadFromJsonAsync<GenreReadDto>();
        Assert.Equal("Action", createdGenre.Name);
    }

    [Fact]
    public async Task UpdateGenre_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/genre", new GenreCreateDto { Name = "Drama" });
        var createdGenre = await createResponse.Content.ReadFromJsonAsync<GenreReadDto>();

        var updated = new GenreCreateDto { Name = "Updated Drama" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/genre/{createdGenre.Id}", updated);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedGenre = await updateResponse.Content.ReadFromJsonAsync<GenreReadDto>();
        Assert.Equal("Updated Drama", updatedGenre.Name);
    }

    [Fact]
    public async Task DeleteGenre_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/genre", new GenreCreateDto { Name = "Temporary" });
        var createdGenre = await createResponse.Content.ReadFromJsonAsync<GenreReadDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/genre/{createdGenre.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await _client.GetAsync($"/api/genre/{createdGenre.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }
}
