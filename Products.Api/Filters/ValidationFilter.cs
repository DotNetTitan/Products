using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Products.Api.Filters;

public sealed class ValidationFilter<T>
    : IAsyncActionFilter
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(
        IValidator<T> validator)
    {
        _validator = validator;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var model = context.ActionArguments
            .Values
            .OfType<T>()
            .FirstOrDefault();

        if (model is null)
        {
            await next();

            return;
        }

        var validationResult =
            await _validator.ValidateAsync(
                model,
                context.HttpContext.RequestAborted);

        if (!validationResult.IsValid)
        {
            context.Result =
                new BadRequestObjectResult(
                    new ValidationProblemDetails(
                        validationResult.ToDictionary()));

            return;
        }

        await next();
    }
}