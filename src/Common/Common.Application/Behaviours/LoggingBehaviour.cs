using Common.Application.Interfaces;
using Common.Application.Services.Logging;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Common.Application.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly ILogger _logger;

        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.Information("Request: {Name} {@Request}", args:
                [requestName, request]);

            return Task.CompletedTask;
        }
    }
}
