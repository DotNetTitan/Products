namespace Products.Application.Features.Products.Responses
{
    public sealed record ProductResponse(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        DateTimeOffset CreatedAtUtc);
}