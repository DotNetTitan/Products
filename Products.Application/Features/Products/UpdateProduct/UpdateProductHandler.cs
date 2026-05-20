using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Products.Application.Abstractions;
using Products.Application.Common.Caching;

namespace Products.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductHandler(IApplicationDbContext dbContext, IMemoryCache memoryCache)
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<bool> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.SetDetails(command.Name, command.Description);

        product.SetPrice(command.Price);

        product.SetStockQuantity(command.StockQuantity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _memoryCache.Remove(CacheKeys.ProductById(product.Id));

        return true;
    }
}