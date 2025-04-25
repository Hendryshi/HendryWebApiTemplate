using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware<T>(this IApplicationBuilder app) where T : class, IMiddleware
        {
            app.UseWhen(
                context =>
                {
                    var pass = true;
                    pass = pass && !context.Request.Path.Value.Equals("/favicon.ico");

                    return pass;
                },
                appBuilder =>
                {
                    appBuilder.UseMiddleware<T>();
                }
            );
            return app;
        }
    }
}
