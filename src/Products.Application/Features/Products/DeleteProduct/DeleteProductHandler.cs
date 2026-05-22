using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Common.Caching;

namespace Products.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler(IApplicationDbContext dbContext, ICacheService cache)
    : ICommandHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cache = cache;

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

        await _cache.RemoveAsync(CacheKeys.ProductById(product.Id), cancellationToken);

        return true;
    }
}