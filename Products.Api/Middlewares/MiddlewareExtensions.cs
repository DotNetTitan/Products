using Products.Api.Middlewares;

namespace Products.Api.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}