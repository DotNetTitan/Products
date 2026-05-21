using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Api.Filters;
using Products.Application.Features.Products.CreateProduct;
using Products.Application.Features.Products.DeleteProduct;
using Products.Application.Features.Products.GetProductById;
using Products.Application.Features.Products.GetProducts;
using Products.Application.Features.Products.UpdateProduct;
using Products.Domain.Enums;

namespace Products.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly CreateProductHandler _createProductHandler;
    private readonly GetProductsHandler _getProductsHandler;
    private readonly GetProductByIdHandler _getProductByIdHandler;
    private readonly UpdateProductHandler _updateProductHandler;
    private readonly DeleteProductHandler _deleteProductHandler;

    public ProductsController(
        CreateProductHandler createProductHandler,
        GetProductsHandler getProductsHandler,
        GetProductByIdHandler getProductByIdHandler,
        UpdateProductHandler updateProductHandler,
        DeleteProductHandler deleteProductHandler)
    {
        _createProductHandler = createProductHandler;
        _getProductsHandler = getProductsHandler;
        _getProductByIdHandler = getProductByIdHandler;
        _updateProductHandler = updateProductHandler;
        _deleteProductHandler = deleteProductHandler;
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    [ServiceFilter(typeof(ValidationFilter<CreateProductCommand>))]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _createProductHandler.Handle(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [ServiceFilter(typeof(ValidationFilter<GetProductsQuery>))]
    public async Task<IActionResult> Get([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
    {
        var result = await _getProductsHandler.Handle(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        var result = await _getProductByIdHandler.Handle(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ServiceFilter(typeof(ValidationFilter<UpdateProductCommand>))]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route ID and body ID must match.");
        }

        var updated = await _updateProductHandler.Handle(command, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        var deleted = await _deleteProductHandler.Handle(command, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}