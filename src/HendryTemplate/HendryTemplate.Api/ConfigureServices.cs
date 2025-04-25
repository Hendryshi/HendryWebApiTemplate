using Common.Domain.Serialization;
using HendryTemplate.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;

namespace HendryTemplate.Api
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();

            services.AddControllers().AddJsonOptions(options =>
            {
                // Adds on startup
                options.JsonSerializerOptions.Converters.Add(new OptionalConverter());
                options.JsonSerializerOptions.WriteIndented = true;
            });

            services.AddScoped<FluentValidationSchemaProcessor>(provider =>
            {
                var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
                var loggerFactory = provider.GetService<ILoggerFactory>();

                return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
            });

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);


            services.AddOpenApiDocument((configure, serviceProvider) =>
            {
                var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();

                // Add the fluent validations schema processor
                configure.SchemaSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);

                configure.Title = SwaggerInfos.ApiTitle;

                // Api version
                var apiVersion = SwaggerInfos.ApiVersion;

                configure.Version = apiVersion;
                configure.DocumentName = SwaggerInfos.DocumentName;

                configure.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Description = "Please keyin valid JWT token into the field",
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
                configure.OperationProcessors.Add(new AddHeadersOperationProcessor());
            });

            return services;
        }

    }
}
