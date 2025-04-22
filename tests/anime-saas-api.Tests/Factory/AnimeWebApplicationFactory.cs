using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AnimeSaasApi.Context;
using System.Linq;


namespace AnimeSaasApi.Tests.Factory
{
    public class AnimeWebApplicationFactory : WebApplicationFactory<AnimeSaasApi.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Supprimer la configuration existante pour AnimeSaasDbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AnimeSaasDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Ajouter un DbContext utilisant InMemory
                services.AddDbContext<AnimeSaasDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAnimeSaasDb");
                });

                // Construire le service provider
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AnimeSaasDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}
