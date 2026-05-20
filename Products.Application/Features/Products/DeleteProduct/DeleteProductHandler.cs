using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Products.Application.Abstractions;
using Products.Application.Common.Caching;

namespace Products.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(
        IApplicationDbContext dbContext,
        IMemoryCache memoryCache,
        ILogger<DeleteProductHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Delete();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _memoryCache.Remove(CacheKeys.ProductById(product.Id));

        _logger.LogInformation("Product soft deleted with ID {ProductId}", product.Id);

        return true;
    }
}