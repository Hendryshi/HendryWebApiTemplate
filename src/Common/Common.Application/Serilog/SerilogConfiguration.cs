using Microsoft.Extensions.Configuration;
using Serilog;

namespace Common.Application.Serilog
{
    public static class SerilogConfiguration
    {
        private static IConfigurationRoot BuildConfiguration(IConfiguration configuration = null)
        {
            bool reloadOnChange = false;
            if (configuration != null && configuration.GetSection("Serilog:ReloadOnChange").Exists())
            {
                try
                {
                    reloadOnChange = configuration.GetValue<bool>("Serilog:ReloadOnChange");
                }
                catch { }
            }

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("serilog.config.json", optional: false, reloadOnChange: reloadOnChange)
                //Env variable can be used to overrride appsettings
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        /// Return a logger wich can be used to log before the DI has been set and so that serilog service is available
        /// </summary>
        /// <returns></returns>
        public static ILogger CreateSerilogLogger(IConfiguration configuration = null)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(BuildConfiguration(configuration))
                .CreateLogger();
        }
    }
}
