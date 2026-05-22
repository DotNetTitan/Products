using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Infrastructure.Data;

namespace Products.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName;

    public CustomWebApplicationFactory()
    {
        _databaseName = Guid.NewGuid().ToString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            services.AddAuthentication(Auth.TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, Auth.TestAuthHandler>(
                    Auth.TestAuthHandler.SchemeName, null);
        });
    }
}
