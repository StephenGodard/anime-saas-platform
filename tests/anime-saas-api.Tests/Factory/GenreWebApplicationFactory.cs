using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AnimeSaasApi;
using AnimeSaasApi.Context;

namespace anime_saas_api.Tests.Factory;

public class GenreWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AnimeSaasDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AnimeSaasDbContext>(options =>
            {
                options.UseInMemoryDatabase("GenreTestDb");
            });
        });

        return base.CreateHost(builder);
    }
}
