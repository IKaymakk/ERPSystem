using ERPSystem.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace ERPSystem.Infrastructure.Registration;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}