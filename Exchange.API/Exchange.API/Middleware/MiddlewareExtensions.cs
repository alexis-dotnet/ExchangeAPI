using Microsoft.AspNetCore.Builder;

namespace Exchange.API.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
