using Common.Application.Exceptions;
using Common.Application.Extensions;
using Common.Application.Services.Logging;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<TRequest> _logger;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators, ILogger<TRequest> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.Debug($"Entering in method {System.Reflection.MethodBase.GetCurrentMethod().Name} of service {this.GetType().Name}");

            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v =>
                        v.ValidateAsync(context, cancellationToken)
                    )
                );

                validationResults.ValidateWithThrow();
            }

            _logger.Debug($"Leaving method {System.Reflection.MethodBase.GetCurrentMethod().Name} of service {this.GetType().Name}");
            return await next();
        }
    }
}
