using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Products.Api.Auth;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAzureAdAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var azureAd = configuration.GetSection("AzureAd");
        var tenantId = azureAd["TenantId"]!;
        var clientId = azureAd["ClientId"]!;

        return services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = $"https://sts.windows.net/{tenantId}/",
                    ValidAudience = clientId
                };
            })
            .Services;
    }
}
