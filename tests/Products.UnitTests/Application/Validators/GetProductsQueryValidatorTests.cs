using FluentValidation.TestHelper;
using Products.Application.Features.Products.GetProducts;

namespace Products.UnitTests.Application.Validators;

public sealed class GetProductsQueryValidatorTests
{
    private readonly GetProductsQueryValidator _validator = new();

    [Fact]
    public void Validate_PageIsZero_HasError()
    {
        var query = new GetProductsQuery(Page: 0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_PageSizeIsZero_HasError()
    {
        var query = new GetProductsQuery(PageSize: 0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_PageSizeExceedsMax_HasError()
    {
        var query = new GetProductsQuery(PageSize: 101);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_InvalidSortBy_HasError()
    {
        var query = new GetProductsQuery(SortBy: "invalid");

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("name")]
    [InlineData("price")]
    [InlineData("NAME")]
    [InlineData("Price")]
    public void Validate_ValidSortBy_NoError(string sortBy)
    {
        var query = new GetProductsQuery(SortBy: sortBy);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public void Validate_NullSortBy_NoError()
    {
        var query = new GetProductsQuery();

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public void Validate_DefaultQuery_NoErrors()
    {
        var query = new GetProductsQuery();

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
