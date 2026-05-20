using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Common.Pagination;
using Products.Application.Features.Products.Responses;

namespace Products.Application.Features.Products.GetProducts;

public sealed class GetProductsHandler
{
    private readonly IApplicationDbContext _dbContext;

    public GetProductsHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<ProductResponse>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Product> productsQuery =
            _dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            productsQuery = productsQuery.Where(x =>
                x.Name.Contains(query.Search));
        }

        productsQuery = query.SortBy?.ToLower() switch
        {
            "price" => query.Descending
                ? productsQuery.OrderByDescending(x => x.Price)
                : productsQuery.OrderBy(x => x.Price),

            "name" => query.Descending
                ? productsQuery.OrderByDescending(x => x.Name)
                : productsQuery.OrderBy(x => x.Name),

            _ => productsQuery.OrderByDescending(x => x.CreatedAtUtc)
        };

        var totalCount =
            await productsQuery.CountAsync(cancellationToken);

        var items = await productsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new ProductResponse(
                x.Id,
                x.Name,
                x.Description,
                x.Price,
                x.StockQuantity,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductResponse>(
            items,
            query.Page,
            query.PageSize,
            totalCount);
    }
}