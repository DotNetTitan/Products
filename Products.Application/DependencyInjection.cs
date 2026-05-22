using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Abstractions;
using Products.Application.Common.Pagination;
using Products.Application.Features.Products.CreateProduct;
using Products.Application.Features.Products.DeleteProduct;
using Products.Application.Features.Products.GetProductById;
using Products.Application.Features.Products.GetProducts;
using Products.Application.Features.Products.Responses;
using Products.Application.Features.Products.UpdateProduct;

namespace Products.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

            services.AddScoped<
                ICommandHandler<CreateProductCommand, CreateProductResponse>,
                CreateProductHandler>();

            services.AddScoped<
                ICommandHandler<UpdateProductCommand, bool>,
                UpdateProductHandler>();

            services.AddScoped<
                ICommandHandler<DeleteProductCommand, bool>,
                DeleteProductHandler>();

            services.AddScoped<
                IQueryHandler<GetProductByIdQuery, ProductResponse?>,
                GetProductByIdHandler>();

            services.AddScoped<
                IQueryHandler<GetProductsQuery, PagedResponse<ProductResponse>>,
                GetProductsHandler>();

            return services;
        }
    }
}