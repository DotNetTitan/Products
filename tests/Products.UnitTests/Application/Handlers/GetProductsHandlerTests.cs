using Microsoft.EntityFrameworkCore;
using Products.Application.Abstractions;
using Products.Application.Features.Products.GetProducts;
using Products.Domain.Entities;
using Products.Infrastructure.Data;

namespace Products.UnitTests.Application.Handlers;

public sealed class GetProductsHandlerTests
{
    private static IApplicationDbContext CreateDbContextWithData()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        context.Products.AddRange(
            new Product("Alpha", "First product", 10, 5),
            new Product("Beta", "Second product", 20, 10),
            new Product("Gamma", "Third product", 30, 15));

        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task Handle_DefaultQuery_ReturnsAllProductsSortedByDateDesc()
    {
        var dbContext = CreateDbContextWithData();
        var handler = new GetProductsHandler(dbContext);

        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithSearch_ReturnsFilteredProducts()
    {
        var dbContext = CreateDbContextWithData();
        var handler = new GetProductsHandler(dbContext);

        var query = new GetProductsQuery(Search: "Beta");

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Beta");
    }

    [Fact]
    public async Task Handle_WithSearchByDescription_ReturnsFilteredProducts()
    {
        var dbContext = CreateDbContextWithData();
        var handler = new GetProductsHandler(dbContext);

        var query = new GetProductsQuery(Search: "Second");

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Beta");
    }

    [Fact]
    public async Task Handle_SortByPriceAscending_ReturnsProductsOrderedByPrice()
    {
        var dbContext = CreateDbContextWithData();
        var handler = new GetProductsHandler(dbContext);

        var query = new GetProductsQuery(SortBy: "price");

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Select(x => x.Price).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_SortByPriceDescending_ReturnsProductsOrderedByPriceDesc()
    {
        var dbContext = CreateDbContextWithData();
        var handler = new GetProductsHandler(dbContext);

        var query = new GetProductsQuery(SortBy: "price", Descending: true);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Select(x => x.Price).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_SortByNameAscending_ReturnsProductsOrderedByName()
    {
        var dbContext = CreateDbContextWithData();
        var handler = new GetProductsHandler(dbContext);

        var query = new GetProductsQuery(SortBy: "name");

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Select(x => x.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        for (int i = 1; i <= 20; i++)
        {
            context.Products.Add(new Product($"Product{i}", $"Desc{i}", i, i));
        }
        context.SaveChanges();

        var handler = new GetProductsHandler(context);

        var query = new GetProductsQuery(Page: 2, PageSize: 5);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(5);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(20);
    }

    [Fact]
    public async Task Handle_SearchNoMatch_ReturnsEmptyList()
    {
        var dbContext = CreateDbContextWithData();
        var handler = new GetProductsHandler(dbContext);

        var query = new GetProductsQuery(Search: "NonExistent");

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
