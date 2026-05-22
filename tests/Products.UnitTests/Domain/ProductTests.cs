using Products.Domain.Entities;

namespace Products.UnitTests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void Constructor_ValidArguments_CreatesProduct()
    {
        var product = new Product("Test", "Description", 10.99m, 5);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Test");
        product.Description.Should().Be("Description");
        product.Price.Should().Be(10.99m);
        product.StockQuantity.Should().Be(5);
        product.IsDeleted.Should().BeFalse();
        product.CreatedAtUtc.Should().Be(default);
        product.UpdatedAtUtc.Should().BeNull();
        product.DeletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void SetDetails_ValidArguments_UpdatesNameAndDescription()
    {
        var product = new Product("Old", "Old desc", 1, 1);

        product.SetDetails("New", "New desc");

        product.Name.Should().Be("New");
        product.Description.Should().Be("New desc");
    }

    [Fact]
    public void SetPrice_ValidPrice_UpdatesPrice()
    {
        var product = new Product("Test", "Desc", 1, 1);

        product.SetPrice(25.99m);

        product.Price.Should().Be(25.99m);
    }

    [Fact]
    public void SetPrice_NegativePrice_ThrowsArgumentException()
    {
        var product = new Product("Test", "Desc", 1, 1);

        Action act = () => product.SetPrice(-1);

        act.Should().Throw<ArgumentException>().WithMessage("Price cannot be negative");
    }

    [Fact]
    public void SetPrice_ZeroPrice_SetsPrice()
    {
        var product = new Product("Test", "Desc", 1, 1);

        product.SetPrice(0);

        product.Price.Should().Be(0);
    }

    [Fact]
    public void SetStockQuantity_ValidQuantity_UpdatesStock()
    {
        var product = new Product("Test", "Desc", 1, 1);

        product.SetStockQuantity(100);

        product.StockQuantity.Should().Be(100);
    }

    [Fact]
    public void SetStockQuantity_NegativeQuantity_ThrowsArgumentException()
    {
        var product = new Product("Test", "Desc", 1, 1);

        Action act = () => product.SetStockQuantity(-1);

        act.Should().Throw<ArgumentException>().WithMessage("Stock quantity cannot be negative");
    }

    [Fact]
    public void SetStockQuantity_ZeroQuantity_SetsQuantity()
    {
        var product = new Product("Test", "Desc", 1, 1);

        product.SetStockQuantity(0);

        product.StockQuantity.Should().Be(0);
    }

    [Fact]
    public void Delete_NotDeleted_SetsIsDeletedAndSetsDeletedAtUtc()
    {
        var product = new Product("Test", "Desc", 1, 1);

        product.Delete();

        product.IsDeleted.Should().BeTrue();
        product.DeletedAtUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Delete_AlreadyDeleted_DoesNotChangeDeletedAtUtc()
    {
        var product = new Product("Test", "Desc", 1, 1);
        product.Delete();
        var originalDeletedAt = product.DeletedAtUtc;

        product.Delete();

        product.DeletedAtUtc.Should().Be(originalDeletedAt);
    }
}
