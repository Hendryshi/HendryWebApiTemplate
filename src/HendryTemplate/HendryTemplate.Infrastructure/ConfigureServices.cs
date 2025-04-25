using Common.Application.Interfaces;
using Common.Infrastructure;
using HendryTemplate.Application.Interfaces;
using HendryTemplate.Infrastructure.Persistence;
using HendryTemplate.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HendryTemplate.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureSqlServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlServices(configuration);
            services.AddCommonInfraServices(configuration);
            return services;
        }

        public static IServiceCollection AddCommonInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCoreInfrastructureCommonServices(configuration);
            services.AddHttpContextAccessor();
            return services;
        }

        public static IServiceCollection AddSqlServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAsyncRepository, BaseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddDbContext<AppDbContext>(options =>
            {
                //sqlserver
                //options.UseSqlServer(configuration.GetConnectionString("DefaultSQLConnection"), m =>
                //{
                //    m.MigrationsHistoryTable("__EFMigrationsHistory", "HendryTemplate");
                //});

                //mysql
                var connectionString = configuration.GetConnectionString("DefaultSQLConnection");
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysqlOptions =>
                    {
                        mysqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
                    }
                );
            });

            services.AddScoped<AppDbContext>();

            return services;
        }
    }
}
