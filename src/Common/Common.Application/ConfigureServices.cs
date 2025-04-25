using Common.Application.Extensions;
using Common.Application.Interfaces;
using Common.Application.Services.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Application
{
    public static class ConfigureServices
    {
        /// <summary>
        /// Must not be called alongside AddCoreApplicationPortalServices.
        /// This method must be called if your api is NOT the Portal. It add the Portal Grpc and services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddCoreApplicationCommonServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddCoreApplicationBaseServices(config);
            return services;
        }

        #region private
        private static IServiceCollection AddCoreApplicationBaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ResultHelper>();

            services.AddScoped<ITokenHelper, TokenHelper>();
            HttpClientHelper.SetHostEnvironment(services.BuildServiceProvider().GetService<IHostEnvironment>());

            return services;
        }

        #endregion

    }
}
