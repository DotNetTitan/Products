using System.Net;
using System.Net.Http.Json;
using Products.Application.Common.Pagination;
using Products.Application.Features.Products.CreateProduct;
using Products.Application.Features.Products.Responses;
using Products.Application.Features.Products.UpdateProduct;

namespace Products.E2ETests;

public sealed class ProductLifecycleTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductLifecycleTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task Scenario_CreateMultipleProducts_ListPaginated_UpdateOne_DeleteAnother()
    {
        var product1 = new CreateProductCommand("Laptop", "High-end laptop", 1499.99m, 10);
        var product2 = new CreateProductCommand("Mouse", "Wireless mouse", 29.99m, 50);
        var product3 = new CreateProductCommand("Keyboard", "Mechanical keyboard", 89.99m, 30);

        var resp1 = await _client.PostAsJsonAsync("/api/products", product1);
        var resp2 = await _client.PostAsJsonAsync("/api/products", product2);
        var resp3 = await _client.PostAsJsonAsync("/api/products", product3);

        resp1.StatusCode.Should().Be(HttpStatusCode.Created);
        resp2.StatusCode.Should().Be(HttpStatusCode.Created);
        resp3.StatusCode.Should().Be(HttpStatusCode.Created);

        var created1 = await resp1.Content.ReadFromJsonAsync<CreateProductResponse>();
        var created2 = await resp2.Content.ReadFromJsonAsync<CreateProductResponse>();
        var created3 = await resp3.Content.ReadFromJsonAsync<CreateProductResponse>();

        var listResponse = await _client.GetAsync("/api/products?page=1&pageSize=10");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await listResponse.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>();
        list!.TotalCount.Should().Be(3);
        list.Items.Should().HaveCount(3);

        var updateCommand = new UpdateProductCommand(
            created1!.Id, "Gaming Laptop", "High-end gaming laptop", 1999.99m, 5);

        var updateResponse = await _client.PutAsJsonAsync($"/api/products/{created1.Id}", updateCommand);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getUpdated = await _client.GetAsync($"/api/products/{created1.Id}");
        var updatedProduct = await getUpdated.Content.ReadFromJsonAsync<ProductResponse>();
        updatedProduct!.Name.Should().Be("Gaming Laptop");
        updatedProduct.Price.Should().Be(1999.99m);
        updatedProduct.StockQuantity.Should().Be(5);

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created2!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDeleted = await _client.GetAsync($"/api/products/{created2.Id}");
        getDeleted.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var getOtherStillExists = await _client.GetAsync($"/api/products/{created3!.Id}");
        getOtherStillExists.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Scenario_SearchAndSortProducts()
    {
        await _client.PostAsJsonAsync("/api/products", new CreateProductCommand("Apple", "Fruit", 1.99m, 100));
        await _client.PostAsJsonAsync("/api/products", new CreateProductCommand("Banana", "Yellow fruit", 0.99m, 200));
        await _client.PostAsJsonAsync("/api/products", new CreateProductCommand("Cherry", "Red fruit", 3.99m, 50));
        await _client.PostAsJsonAsync("/api/products", new CreateProductCommand("Date", "Sweet fruit", 5.99m, 30));

        var searchResponse = await _client.GetAsync("/api/products?search=Apple&pageSize=10");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>();
        searchResult!.Items.Should().HaveCount(1);
        searchResult.Items.First().Name.Should().Be("Apple");

        var sortedResponse = await _client.GetAsync("/api/products?sortBy=price&descending=true&pageSize=10");
        sortedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var sortedResult = await sortedResponse.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>();
        sortedResult!.Items.Select(x => x.Price).Should().BeInDescendingOrder();

        var sortedAscResponse = await _client.GetAsync("/api/products?sortBy=name&pageSize=10");
        sortedAscResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var sortedAscResult = await sortedAscResponse.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>();
        sortedAscResult!.Items.Select(x => x.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Scenario_ValidationErrors_ReturnBadRequest()
    {
        var emptyName = new CreateProductCommand("", "Desc", 10, 5);
        var response1 = await _client.PostAsJsonAsync("/api/products", emptyName);
        response1.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var negativePrice = new CreateProductCommand("Name", "Desc", -1, 5);
        var response2 = await _client.PostAsJsonAsync("/api/products", negativePrice);
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var invalidPageSize = await _client.GetAsync("/api/products?page=1&pageSize=200");
        invalidPageSize.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Scenario_ConcurrentCreate_ReturnsUniqueProducts()
    {
        var tasks = Enumerable.Range(1, 10).Select(i =>
            _client.PostAsJsonAsync("/api/products",
                new CreateProductCommand($"Concurrent Product {i}", $"Test {i}", i * 10, i)));

        var responses = await Task.WhenAll(tasks);

        responses.All(r => r.StatusCode == HttpStatusCode.Created).Should().BeTrue();

        var listResponse = await _client.GetAsync("/api/products?pageSize=100");
        var list = await listResponse.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>();
        list!.Items.Should().HaveCount(10);
    }
}
