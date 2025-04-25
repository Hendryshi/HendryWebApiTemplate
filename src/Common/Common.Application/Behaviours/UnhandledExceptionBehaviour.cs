using Common.Application.Services.Logging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Debug($"Entering in method {System.Reflection.MethodBase.GetCurrentMethod().Name} of service {this.GetType().Name}");
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;

                _logger.Error(ex, "Unhandled Exception for Request {Name} {@Request}", args: [requestName, request]);

                throw;
            }
        }
    }
}
