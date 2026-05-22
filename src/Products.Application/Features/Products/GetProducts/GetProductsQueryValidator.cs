using FluentValidation;

namespace Products.Application.Features.Products.GetProducts;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
            .WithMessage("SortBy must be one of: name, price");
    }

    private static bool BeValidSortField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return true;
        }

        return string.Equals(sortBy, "name", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(sortBy, "price", StringComparison.OrdinalIgnoreCase);
    }
}