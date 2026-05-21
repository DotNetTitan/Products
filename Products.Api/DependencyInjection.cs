using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Products.Api.Auth.Login;
using Products.Api.Auth.Token;
using Products.Api.Filters;
using Products.Domain.Entities;

namespace Products.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddScoped(typeof(ValidationFilter<>));

            services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();

            services.AddScoped<JwtTokenGenerator>();

            services.AddScoped<LoginService>();

            return services;
        }
    }
}