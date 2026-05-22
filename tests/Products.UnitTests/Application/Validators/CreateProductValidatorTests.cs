using FluentValidation.TestHelper;
using Products.Application.Features.Products.CreateProduct;

namespace Products.UnitTests.Application.Validators;

public sealed class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator = new();

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateProductCommand("", "Desc", 10, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameExceedsMaxLength_HasError()
    {
        var command = new CreateProductCommand(new string('a', 201), "Desc", 10, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_EmptyDescription_HasError()
    {
        var command = new CreateProductCommand("Name", "", 10, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_DescriptionExceedsMaxLength_HasError()
    {
        var command = new CreateProductCommand("Name", new string('a', 2001), 10, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_NegativePrice_HasError()
    {
        var command = new CreateProductCommand("Name", "Desc", -1, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_NegativeStockQuantity_HasError()
    {
        var command = new CreateProductCommand("Name", "Desc", 10, -1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateProductCommand("Valid Name", "Valid description", 9.99m, 5);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
