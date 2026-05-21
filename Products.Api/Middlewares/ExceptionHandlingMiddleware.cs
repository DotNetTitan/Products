using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Products.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            await WriteProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            await WriteProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Operation Failed",
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await WriteProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "Server Error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;

        context.Response.ContentType =
            "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        var json = JsonSerializer.Serialize(problemDetails);

        await context.Response.WriteAsync(json);
    }
}