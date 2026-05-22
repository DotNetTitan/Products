using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Common.Caching;

namespace Products.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductHandler(IApplicationDbContext dbContext, ICacheService cache)
    : ICommandHandler<UpdateProductCommand, bool>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cache = cache;

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

        await _cache.RemoveAsync(CacheKeys.ProductById(product.Id), cancellationToken);

        return true;
    }
}