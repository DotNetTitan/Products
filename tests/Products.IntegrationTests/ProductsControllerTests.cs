using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Features.Products.CreateProduct;
using Products.Application.Features.Products.Responses;
using Products.Application.Features.Products.UpdateProduct;
using Products.Application.Common.Pagination;

namespace Products.IntegrationTests;

public sealed class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_Returns201Created()
    {
        var command = new CreateProductCommand("Integration Test", "Created via integration test", 49.99m, 100);

        var response = await _client.PostAsJsonAsync("/api/products", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProduct_EmptyName_Returns400BadRequest()
    {
        var command = new CreateProductCommand("", "Desc", 10, 5);

        var response = await _client.PostAsJsonAsync("/api/products", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_Returns200Ok()
    {
        var create = new CreateProductCommand("GetById Test", "Testing GetById", 15, 3);
        var createResponse = await _client.PostAsJsonAsync("/api/products", create);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();

        var response = await _client.GetAsync($"/api/products/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        product.Should().NotBeNull();
        product!.Name.Should().Be("GetById Test");
    }

    [Fact]
    public async Task GetProductById_NonExistentProduct_Returns404NotFound()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProducts_DefaultQuery_Returns200OkWithPagedResponse()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateProduct_ExistingProduct_Returns204NoContent()
    {
        var create = new CreateProductCommand("Before Update", "Will be updated", 10, 5);
        var createResponse = await _client.PostAsJsonAsync("/api/products", create);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();

        var command = new UpdateProductCommand(created!.Id, "After Update", "Updated description", 25, 50);

        var response = await _client.PutAsJsonAsync($"/api/products/{created.Id}", command);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateProduct_RouteIdMismatch_Returns400BadRequest()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Name", "Desc", 10, 5);

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_NonExistentProduct_Returns404NotFound()
    {
        var id = Guid.NewGuid();
        var command = new UpdateProductCommand(id, "Name", "Desc", 10, 5);

        var response = await _client.PutAsJsonAsync($"/api/products/{id}", command);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_ExistingProduct_Returns204NoContent()
    {
        var create = new CreateProductCommand("To Delete", "Will be deleted", 10, 5);
        var createResponse = await _client.PostAsJsonAsync("/api/products", create);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();

        var response = await _client.DeleteAsync($"/api/products/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProduct_NonExistentProduct_Returns404NotFound()
    {
        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FullLifecycle_CreateGetUpdateDelete_Succeeds()
    {
        var create = new CreateProductCommand("Lifecycle", "Full lifecycle test", 100, 10);
        var createResponse = await _client.PostAsJsonAsync("/api/products", create);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();

        var getResponse = await _client.GetAsync($"/api/products/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var update = new UpdateProductCommand(created.Id, "Lifecycle Updated", "Updated", 200, 20);
        var updateResponse = await _client.PutAsJsonAsync($"/api/products/{created.Id}", update);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDeletedResponse = await _client.GetAsync($"/api/products/{created.Id}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
