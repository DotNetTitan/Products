using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Common.Caching;
using Products.Application.Features.Products.Responses;

namespace Products.Application.Features.Products.GetProductById;

public sealed class GetProductByIdHandler(IApplicationDbContext dbContext, ICacheService cache)
    : IQueryHandler<GetProductByIdQuery, ProductResponse?>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cache = cache;

    public async Task<ProductResponse?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductById(query.Id);

        var cachedProduct = await _cache.GetAsync<ProductResponse>(cacheKey, cancellationToken);

        if (cachedProduct is not null)
        {
            return cachedProduct;
        }

        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .Select(x => new ProductResponse(
                x.Id,
                x.Name,
                x.Description,
                x.Price,
                x.StockQuantity,
                x.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return null;
        }

        await _cache.SetAsync(cacheKey, product, CacheDurations.Product, cancellationToken);

        return product;
    }
}