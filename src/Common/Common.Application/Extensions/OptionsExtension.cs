using Common.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Application.Extensions
{
    public static class OptionsExtension
    {
        public static T GetOptions<T>(this IServiceCollection services, Action<T> options)
            where T : class, IOptionsFile
        {
            services.Configure(options);
            return services.BuildServiceProvider().GetRequiredService<IOptions<T>>().Value;
        }
        public static T GetOptions<T>(this IServiceCollection services)
            where T : class, IOptionsFile
        {
            return services.BuildServiceProvider().GetRequiredService<IOptions<T>>().Value;
        }
    }
}
