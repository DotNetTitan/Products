using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Features.Products.CreateProduct;
using Products.Application.Features.Products.DeleteProduct;
using Products.Application.Features.Products.GetProductById;
using Products.Application.Features.Products.GetProducts;
using Products.Application.Features.Products.UpdateProduct;

namespace Products.Application
{
    public static class DependencyInjection
    {
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