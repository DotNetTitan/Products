using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Features.Products.CreateProduct;
using Products.Domain.Entities;
using Products.Infrastructure.Data;

namespace Products.UnitTests.Application.Handlers;

public sealed class CreateProductHandlerTests
{
    private static IApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesProductAndReturnsResponse()
    {
        var dbContext = CreateDbContext();
        var handler = new CreateProductHandler(dbContext);
        var command = new CreateProductCommand("Test Product", "A test product", 19.99m, 10);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        var product = await dbContext.Products.FindAsync(result.Id);
        product.Should().NotBeNull();
        product!.Name.Should().Be("Test Product");
        product.Description.Should().Be("A test product");
        product.Price.Should().Be(19.99m);
        product.StockQuantity.Should().Be(10);
    }
}
