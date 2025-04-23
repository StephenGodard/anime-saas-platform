using AnimeSaasApi;
using AnimeSaasApi.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace anime_saas_api.Tests.Factory;

public class UserWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Supprimer le DbContext existant
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AnimeSaasDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Ajouter un DbContext InMemory
            services.AddDbContext<AnimeSaasDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryUserDb");
            });

            // Initialiser la base de données
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AnimeSaasDbContext>();
            db.Database.EnsureDeleted(); // Supprime la base de données existante
            db.Database.EnsureCreated();
        });
    }
}
