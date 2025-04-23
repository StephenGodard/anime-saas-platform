using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AnimeSaasApi.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using anime_saas_api.Tests.Factory;
using System.Text.Json;

namespace anime_saas_api.Tests;

public class StudioControllerTests : IClassFixture<StudioWebApplicationFactory>
{
    private readonly StudioWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public StudioControllerTests(StudioWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateStudio_ReturnsCreated()
    {
        var studioDto = new StudioCreateDto { Name = "Madhouse", LogoUrl = "https://www.cineanimation.fr/sites/default/files/styles/400xauto/public/2020-12/Madhouse.jpg" };

        var response = await _client.PostAsJsonAsync("/api/studio", studioDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdStudio = await response.Content.ReadFromJsonAsync<StudioReadDto>();
        Assert.Equal("Madhouse", createdStudio.Name);
        Assert.Equal("https://www.cineanimation.fr/sites/default/files/styles/400xauto/public/2020-12/Madhouse.jpg", createdStudio.LogoUrl);
    }

    [Fact]
    public async Task UpdateStudio_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/studio", new StudioCreateDto { Name = "Wit Studio", LogoUrl = "https://wit.png" });
        var  createdStudio = await createResponse.Content.ReadFromJsonAsync<StudioReadDto>();


        var updated = new StudioCreateDto { Name = "WIT", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/a/af/Wit_studio.png" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/studio/{createdStudio.Id}", updated);


        var  updatedStudio = await updateResponse.Content.ReadFromJsonAsync<StudioReadDto>();
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        Assert.Equal("WIT", updatedStudio.Name);
        Assert.Equal("https://upload.wikimedia.org/wikipedia/commons/a/af/Wit_studio.png", updatedStudio.LogoUrl);
    }

    [Fact]
    public async Task DeleteStudio_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/studio", new StudioCreateDto { Name = "Bones", LogoUrl = "https://sm.ign.com/t/ign_in/screenshot/default/studio-bones_mqn1.1280.png" });
        var createdStudio = await createResponse.Content.ReadFromJsonAsync<StudioReadDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/studio/{createdStudio.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await _client.GetAsync($"/api/studio/{createdStudio.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }
}
