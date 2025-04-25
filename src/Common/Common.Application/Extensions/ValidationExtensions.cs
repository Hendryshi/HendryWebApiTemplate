using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;

namespace Common.Application.Extensions
{
    public static class ValidationExtensions
    {

        public static IRuleBuilder<T, string> IsValidLogLevel<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder.Must(x => Enum.TryParse(x, ignoreCase: true, out LogEventLevel level) && Enum.IsDefined(typeof(LogEventLevel), level)).WithMessage("API-ERROR.CORE.VALUE-NOT-VALID-LOGLEVEL");

        public static IRuleBuilderOptionsConditions<TSource, TProperty> MapAndSetValidator<TSource, TDestination, TProperty, TValidator>(
             this IRuleBuilder<TSource, TProperty> ruleBuilder,
             IServiceProvider serviceProvider
         )
            where TValidator : AbstractValidator<TDestination>
        {
            return ruleBuilder.CustomAsync(async (propertyValue, context, cancellationToken) =>
            {
                // Resolve IMapper from the service provider
                var mapper = serviceProvider.GetRequiredService<IMapper>();

                // Map the property value (TProperty) to the destination type (TDestination)
                var mappedObject = mapper.Map<TDestination>(propertyValue);

                // Create an instance of the validator using ActivatorUtilities
                var validator = ActivatorUtilities.CreateInstance<TValidator>(serviceProvider);

                // Validate the mapped object using the validator
                var validationResult = await validator.ValidateAsync(mappedObject);

                if(!validationResult.IsValid)
                {
                    // Add each validation failure to the context
                    foreach(var failure in validationResult.Errors)
                    {
                        context.AddFailure(failure.PropertyName, failure.ErrorMessage);
                    }
                }
            });
        }

    }
}
