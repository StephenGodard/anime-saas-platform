using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AnimeSaasApi;
using AnimeSaasApi.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace anime_saas_api.Tests.Factory;
    public class UserControllerTests : IClassFixture<UserWebApplicationFactory>
    {
        private readonly HttpClient _client;
                public UserControllerTests(UserWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/api/user");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetUserById_ReturnsSuccessStatusCode()
        {
            // Création d’un utilisateur avant de tester la récupération
            var newUser = new
            {
                Username = $"testuser{Guid.NewGuid()}",
                Email = $"testuser{Guid.NewGuid()}@example.com",
                Password = "StrongPassword123!"
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/user", content);
            createResponse.EnsureSuccessStatusCode();

            // Ensuite on teste le GET par ID
            var response = await _client.GetAsync("/api/user/1");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedStatusCode()
        {
            var newUser = new
            {
                Username = $"testuser{Guid.NewGuid()}",
                Email = $"testuser{Guid.NewGuid()}@example.com",
                Password = "StrongPassword123!"
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/user", content);

            var contentResult =  await response.Content.ReadAsStringAsync();
            Console.WriteLine(contentResult);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent()
        {
            var newUser = new
            {
                Username = "deletetestuser",
                Email = "deleteuser@example.com",
                Password = "DeletePass789!"
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/user", content);
            createResponse.EnsureSuccessStatusCode();

            var response = await _client.DeleteAsync("/api/user/1");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk()
        {
            // Créer un utilisateur
            var newUser = new
            {
                Username = "originaluser",
                Email = "original@example.com",
                Password = "InitialPass123!"
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/user", content);
            createResponse.EnsureSuccessStatusCode();

            // Modifier l'utilisateur
            var updatedUser = new
            {
                Username = "updateduser",
                Email = "updated@example.com",
                Password = "UpdatedPass456!"
            };

            var updateContent = new StringContent(JsonSerializer.Serialize(updatedUser), Encoding.UTF8, "application/json");
            var updateResponse = await _client.PutAsync("/api/user/1", updateContent);

            Assert.Equal(System.Net.HttpStatusCode.OK, updateResponse.StatusCode);
        }
    }