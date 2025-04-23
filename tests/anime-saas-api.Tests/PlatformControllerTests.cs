using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AnimeSaasApi.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using anime_saas_api.Tests.Factory;

namespace anime_saas_api.Tests;

public class PlatformControllerTests : IClassFixture<PlatformWebApplicationFactory>
{
    private readonly PlatformWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PlatformControllerTests(PlatformWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreatePlatform_ReturnsCreated()
    {
        var dto = new PlatformCreateDto { Name = "Netflix" };

        var response = await _client.PostAsJsonAsync("/api/platform", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<PlatformReadDto>();
        Assert.Equal("Netflix", created.Name);
    }

    [Fact]
    public async Task UpdatePlatform_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/platform", new PlatformCreateDto { Name = "ADN" });
        var created = await createResponse.Content.ReadFromJsonAsync<PlatformReadDto>();

        var updateDto = new PlatformCreateDto { Name = "ADN Plus" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/platform/{created.Id}", updateDto);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<PlatformReadDto>();
        Assert.Equal("ADN Plus", updated.Name);
    }

    [Fact]
    public async Task DeletePlatform_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/platform", new PlatformCreateDto { Name = "Crunchyroll" });
        var created = await createResponse.Content.ReadFromJsonAsync<PlatformReadDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/platform/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await _client.GetAsync($"/api/platform/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }
}
