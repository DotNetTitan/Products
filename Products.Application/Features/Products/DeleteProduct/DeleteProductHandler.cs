using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Products.Application.Abstractions;
using Products.Application.Common.Caching;

namespace Products.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler(IApplicationDbContext dbContext, IMemoryCache memoryCache)
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Delete();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _memoryCache.Remove(CacheKeys.ProductById(product.Id));

        return true;
    }
}