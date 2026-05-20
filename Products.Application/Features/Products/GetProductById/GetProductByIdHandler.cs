using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Products.Application.Abstractions;
using Products.Application.Common.Caching;
using Products.Application.Features.Products.Responses;

namespace Products.Application.Features.Products.GetProductById;

public sealed class GetProductByIdHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public GetProductByIdHandler(IApplicationDbContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    public async Task<ProductResponse?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductById(query.Id);

        if (_memoryCache.TryGetValue(cacheKey, out ProductResponse? cachedProduct))
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

        _memoryCache.Set(cacheKey, product, CacheDurations.Product);

        return product;
    }
}