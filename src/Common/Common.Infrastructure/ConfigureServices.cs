using Common.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Net;
using System.Text;

namespace Common.Infrastructure
{
    public static class ConfigureServices
    {
        /// <summary>
        /// Must not be called alongside AddCoreInfrastructurePortalServices.
        /// This method must be called if your api is NOT the Portal. It add the Portal Grpc and services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCoreInfrastructureCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration);
            services.AddCoreInfraCommonServices();

            return services;
        }

        #region private
        private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("jwt");
            if(!section.Exists())
                throw new System.ArgumentException("Missing jwt configuration in appsettings.json", nameof(section));

            var tokenKey = section.GetValue<string>("SecretKey") ?? throw new ArgumentNullException("jwt key not found");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    };

                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Log.Error("Authentication failed: " + context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Log.Information("Token validated");
                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            Log.Warning("Token challenge failed.");
                            context.HandleResponse(); // This stops the pipeline

                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"error\": \"Unauthorized - Token validation failed\"}");
                        }
                    };
                });

            services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());
        }

        private static IServiceCollection AddCoreInfraCommonServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<EntitySaveChangesInterceptor>();

            return services;
        }

        #endregion

    }
}
