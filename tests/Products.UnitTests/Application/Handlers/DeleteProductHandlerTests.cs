using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Features.Products.DeleteProduct;
using Products.Domain.Entities;
using Products.Infrastructure.Data;

namespace Products.UnitTests.Application.Handlers;

public sealed class DeleteProductHandlerTests
{
    private static (IApplicationDbContext, ICacheService) CreateDependencies()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return (new ApplicationDbContext(options), new TestCacheService());
    }

    [Fact]
    public async Task Handle_ExistingProduct_SoftDeletesAndReturnsTrue()
    {
        var (dbContext, cache) = CreateDependencies();
        var handler = new DeleteProductHandler(dbContext, cache);
        var product = new Product("Test", "Desc", 10, 5);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCommand(product.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        var deleted = await dbContext.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == product.Id);
        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
        deleted.DeletedAtUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsFalse()
    {
        var (dbContext, cache) = CreateDependencies();
        var handler = new DeleteProductHandler(dbContext, cache);

        var command = new DeleteProductCommand(Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
