//using Common.Application.Exceptions;
//using Common.Application.Extensions;
//using Common.Application.Interfaces;
//using Common.Application.Services.Logging;
//using Common.Domain.Multitenancy;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Serilog;
//using Serilog.Context;

//namespace Common.Application.Services.Middleware
//{
//    public class MultiTenantMiddleware : IMiddleware
//    {
//        public static readonly string TenantHeaderName = "X-TenantName";

//        private readonly IIdentityService _service;
//        private readonly ILogger<MultiTenantMiddleware> _logger;
//        private readonly ICoreExecutionContext _coreExecutionContext;
//        private readonly IDiagnosticContext _diagnosticContext;

//        public MultiTenantMiddleware(IIdentityService service, ILogger<MultiTenantMiddleware> logger,
//            ICoreExecutionContext coreExecutionContext, IDiagnosticContext diagnosticContext = null)
//        {
//            _service = service;
//            _logger = logger;
//            _coreExecutionContext = coreExecutionContext;
//            _diagnosticContext = diagnosticContext;
//        }

//        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//        {
//            _logger.Debug($"Entering {this.GetType().Name}");

//            Tenant tenant = null;
//            TenantConfiguration tenantConfig = null;
//            if (context.Request.Headers.ContainsKey(TenantHeaderName))
//            {
//                string tenantName = context.Request.Headers[TenantHeaderName];
//                _logger.DebugSecurity($"Tenant name requested: {tenantName}");

//                if (!string.IsNullOrWhiteSpace(tenantName))
//                {
//                    var tenantNameString = tenantName.ToString();

//                    var getTenant = await _service.GetTenantByName(tenantNameString);
//                    if (getTenant.IsFailed)
//                    {
//                        var errMsg = $"Failed to find tenant name.Tenant name requested: {tenantName}\n{getTenant.ToErrorMessages()}";
//                        _logger.ErrorSecurity(errMsg);
//                        throw new NotFoundException(errMsg);
//                    }
//                    else
//                    {
//                        tenant = getTenant.Value;
//                        if (tenant == null)
//                        {
//                            var errMsg = "Invalid Tenant Name";
//                            _logger.ErrorSecurity(errMsg);
//                            throw new ArgumentException(errMsg);
//                        }
//                    }
//                }
//            }

//            _coreExecutionContext.SetTenant(tenant);

//            tenantConfig = await _service.GetTenantConfigurationById(tenant.Id.Value);
//            _coreExecutionContext.SetTenantConfiguration(tenantConfig);

//            // Add custom properties to the diagnostic context
//            _diagnosticContext?.Set("TenantId", tenant.Id);

//            _logger.Debug($"Leaving {this.GetType().Name}");
//            using (LogContext.PushProperty("TenantId", tenant.Id))
//            {
//                await next.Invoke(context);
//            }
//        }
//    }

//    public static class TenantMiddlewareExtensions
//    {
//        public static IApplicationBuilder UseMultiTenant(this IApplicationBuilder builder)
//        {
//            return builder.UseCustomMiddleware<MultiTenantMiddleware>();
//        }
//    }
//}
