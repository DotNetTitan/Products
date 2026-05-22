using FluentValidation;
using Products.Api.Filters;

namespace Products.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddScoped(typeof(ValidationFilter<>));

            return services;
        }
    }
}