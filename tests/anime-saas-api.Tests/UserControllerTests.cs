using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AnimeSaasApi;
using AnimeSaasApi.Context;
using AnimeSaasApi.Dtos;
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
            // Arrange & Act
            var response = await _client.GetAsync("/api/user");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }

        [Fact]
        public async Task GetUserById_ReturnsSuccessStatusCode()
        {
            // Arrange - Création d'un utilisateur avant de tester la récupération
            var username = $"testuser{Guid.NewGuid()}";
            var email = $"testuser{Guid.NewGuid()}@example.com";
            
            var newUser = new
            {
                Username = username,
                Email = email,
                Password = "StrongPassword123!"
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/user", content);
            createResponse.EnsureSuccessStatusCode();
            
            // Récupérer l'ID de l'utilisateur créé
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createdUser = JsonSerializer.Deserialize<UserReadDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Act
            var response = await _client.GetAsync($"/api/user/{createdUser.Id}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var getUserContent = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserReadDto>(getUserContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.Equal(username, user?.Username);
            Assert.Equal(email, user?.Email);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedStatusCode()
        {
            // Arrange
            var username = $"testuser{Guid.NewGuid()}";
            var email = $"testuser{Guid.NewGuid()}@example.com";
            
            var newUser = new
            {
                Username = username,
                Email = email,
                Password = "StrongPassword123!"
            };

            // Act
            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/user", content);
            
            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var contentResult = await response.Content.ReadAsStringAsync();
            var createdUser = JsonSerializer.Deserialize<UserReadDto>(contentResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(createdUser);
            Assert.Equal(username, createdUser?.Username);
            Assert.Equal(email, createdUser?.Email);
            Assert.NotEqual(default, createdUser?.DateCreated);
        }
        
        [Fact]
        public async Task CreateUser_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var invalidUser = new
            {
                Username = "u", // Trop court (min 3 caractères)
                Email = "invalid-email", // Email invalide
                Password = "weak" // Mot de passe trop faible
            };
            
            // Act
            var content = new StringContent(JsonSerializer.Serialize(invalidUser), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/user", content);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var email = $"duplicate{Guid.NewGuid()}@example.com";
            
            var firstUser = new
            {
                Username = $"firstuser{Guid.NewGuid()}",
                Email = email,
                Password = "StrongPassword123!"
            };
            
            var duplicateUser = new
            {
                Username = $"seconduser{Guid.NewGuid()}",
                Email = email, // Même email
                Password = "StrongPassword123!"
            };
            
            // Act - Premier utilisateur
            var content1 = new StringContent(JsonSerializer.Serialize(firstUser), Encoding.UTF8, "application/json");
            await _client.PostAsync("/api/user", content1);
            
            // Act - Deuxième utilisateur avec même email
            var content2 = new StringContent(JsonSerializer.Serialize(duplicateUser), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/user", content2);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Email already exists", responseContent);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent()
        {
            // Arrange - Créer un utilisateur à supprimer
            var newUser = new
            {
                Username = $"deleteuser{Guid.NewGuid()}",
                Email = $"deleteuser{Guid.NewGuid()}@example.com",
                Password = "DeletePass789!"
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/user", content);
            createResponse.EnsureSuccessStatusCode();
            
            // Récupérer l'ID de l'utilisateur créé
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createdUser = JsonSerializer.Deserialize<UserReadDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Act
            var response = await _client.DeleteAsync($"/api/user/{createdUser.Id}");
            
            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            
            // Vérifier que l'utilisateur a bien été supprimé
            var getResponse = await _client.GetAsync($"/api/user/{createdUser.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk()
        {
            // Arrange - Créer un utilisateur
            var originalUsername = $"original{Guid.NewGuid()}";
            var originalEmail = $"original{Guid.NewGuid()}@example.com";
            
            var newUser = new
            {
                Username = originalUsername,
                Email = originalEmail,
                Password = "InitialPass123!"
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/user", content);
            createResponse.EnsureSuccessStatusCode();
            
            // Récupérer l'ID de l'utilisateur créé
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createdUser = JsonSerializer.Deserialize<UserReadDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Modifier l'utilisateur
            var updatedUsername = $"updated{Guid.NewGuid()}";
            var updatedEmail = $"updated{Guid.NewGuid()}@example.com";
            
            var updatedUser = new
            {
                Username = updatedUsername,
                Email = updatedEmail,
                Password = "UpdatedPass456!"
            };

            // Act
            var updateContent = new StringContent(JsonSerializer.Serialize(updatedUser), Encoding.UTF8, "application/json");
            var updateResponse = await _client.PutAsync($"/api/user/{createdUser.Id}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            
            // Vérifier que les données ont bien été mises à jour
            var getResponse = await _client.GetAsync($"/api/user/{createdUser.Id}");
            getResponse.EnsureSuccessStatusCode();
            
            var getUserContent = await getResponse.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserReadDto>(getUserContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.Equal(updatedUsername, user?.Username);
            Assert.Equal(updatedEmail, user?.Email);
            Assert.NotEqual(originalUsername, user?.Username);
            Assert.NotEqual(originalEmail, user?.Email);
        }
        
        [Fact]
        public async Task GetNonExistingUser_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/user/99999");
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
    