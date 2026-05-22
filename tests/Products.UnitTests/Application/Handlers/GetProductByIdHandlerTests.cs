using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Features.Products.GetProductById;
using Products.Domain.Entities;
using Products.Infrastructure.Data;

namespace Products.UnitTests.Application.Handlers;

public sealed class GetProductByIdHandlerTests
{
    private static (IApplicationDbContext, ICacheService) CreateDependencies()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return (new ApplicationDbContext(options), new TestCacheService());
    }

    [Fact]
    public async Task Handle_ExistingProduct_ReturnsProduct()
    {
        var (dbContext, cache) = CreateDependencies();
        var product = new Product("Test", "Desc", 10, 5);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductByIdHandler(dbContext, cache);

        var query = new GetProductByIdQuery(product.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be("Test");
        result.Description.Should().Be("Desc");
        result.Price.Should().Be(10);
        result.StockQuantity.Should().Be(5);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNull()
    {
        var (dbContext, cache) = CreateDependencies();

        var handler = new GetProductByIdHandler(dbContext, cache);

        var query = new GetProductByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ProductCached_ReturnsFromCache()
    {
        var (dbContext, cache) = CreateDependencies();
        var product = new Product("Test", "Desc", 10, 5);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductByIdHandler(dbContext, cache);

        var query = new GetProductByIdQuery(product.Id);

        var firstResult = await handler.Handle(query, CancellationToken.None);

        var cachedResult = await handler.Handle(query, CancellationToken.None);

        cachedResult.Should().NotBeNull();
        cachedResult!.Id.Should().Be(product.Id);
        cachedResult.Name.Should().Be("Test");
    }
}
