using FluentValidation.TestHelper;
using Products.Application.Features.Products.UpdateProduct;

namespace Products.UnitTests.Application.Validators;

public sealed class UpdateProductValidatorTests
{
    private readonly UpdateProductValidator _validator = new();

    [Fact]
    public void Validate_EmptyId_HasError()
    {
        var command = new UpdateProductCommand(Guid.Empty, "Name", "Desc", 10, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "", "Desc", 10, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NegativePrice_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Name", "Desc", -1, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_NegativeStock_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Name", "Desc", 10, -1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Name", "Desc", 10, 5);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
