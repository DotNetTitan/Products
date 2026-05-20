using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Abstractions;
using Products.Application.Features.Products.CreateProduct;
using Products.Application.Features.Products.DeleteProduct;
using Products.Application.Features.Products.GetProductById;
using Products.Application.Features.Products.GetProducts;
using Products.Application.Features.Products.UpdateProduct;
using Products.Infrastructure.Data;

namespace Products.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

            services.AddScoped<CreateProductHandler>();
            services.AddScoped<GetProductsHandler>();
            services.AddScoped<GetProductByIdHandler>();
            services.AddScoped<UpdateProductHandler>();
            services.AddScoped<DeleteProductHandler>();

            return services;
        }
    }
}