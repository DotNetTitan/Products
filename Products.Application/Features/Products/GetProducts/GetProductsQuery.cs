namespace Products.Application.Features.Products.GetProducts
{
    public sealed record GetProductsQuery(
        int Page = 1,
        int PageSize = 10,
        string? Search = null,
        string? SortBy = null,
        bool Descending = false);
}