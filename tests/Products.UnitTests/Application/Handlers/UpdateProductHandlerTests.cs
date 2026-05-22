using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Features.Products.UpdateProduct;
using Products.Domain.Entities;
using Products.Infrastructure.Data;

namespace Products.UnitTests.Application.Handlers;

public sealed class UpdateProductHandlerTests
{
    private static (IApplicationDbContext, ICacheService) CreateDependencies()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return (new ApplicationDbContext(options), new TestCacheService());
    }

    [Fact]
    public async Task Handle_ExistingProduct_UpdatesAndReturnsTrue()
    {
        var (dbContext, cache) = CreateDependencies();
        var handler = new UpdateProductHandler(dbContext, cache);
        var product = new Product("Old", "Old desc", 10, 5);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(product.Id, "New", "New desc", 25, 20);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        var updated = await dbContext.Products.FindAsync(product.Id);
        updated!.Name.Should().Be("New");
        updated.Description.Should().Be("New desc");
        updated.Price.Should().Be(25);
        updated.StockQuantity.Should().Be(20);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsFalse()
    {
        var (dbContext, cache) = CreateDependencies();
        var handler = new UpdateProductHandler(dbContext, cache);

        var command = new UpdateProductCommand(Guid.NewGuid(), "New", "Desc", 10, 5);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
