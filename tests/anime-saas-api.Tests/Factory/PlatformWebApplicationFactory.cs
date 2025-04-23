using System.Linq;
using AnimeSaasApi;
using AnimeSaasApi.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace anime_saas_api.Tests.Factory;

public class PlatformWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AnimeSaasDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AnimeSaasDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryPlatformDb");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AnimeSaasDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
