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

public class RecommendationControllerTests : IClassFixture<RecommendationWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RecommendationControllerTests(RecommendationWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnimeSaasApi.Context.AnimeSaasDbContext>();

        db.Recommendations.RemoveRange(db.Recommendations); // Vide la table avant d'ajouter
        db.SaveChanges();

        db.Recommendations.Add(new Recommendation
        {
            UserId = 1,
            AnimeId = 1,
            Score = 75
        });
        db.SaveChanges();
    }


/* A finaliser après completion du ml service
    [Fact]
    public async Task GetRecommendations_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/recommendation/1");
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateRecommendations_ReturnsNoContent()
    {
        var response = await _client.PostAsync("/api/recommendation/update/1", null);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
    */
}
