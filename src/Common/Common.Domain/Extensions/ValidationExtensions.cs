using Common.Domain.Common;
using FluentValidation;

namespace Common.Domain.Extensions
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, int code, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                rule.WithMessage(message);
            }
            return rule.WithErrorCode(code.ToString());
        }

        public static IRuleBuilder<T, string> IsFormatGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder.Must(x => Guid.TryParse(x, out _)).WithMessage("API-ERROR.CORE.VALUE-NOT-GUID");

        /// <summary>
        /// Sets a validator on the Value property of an Optional<TType> parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TValidation"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, Optional<TType>> SetValidator<T, TType, TValidation>(this IRuleBuilder<T, Optional<TType>> ruleBuilder, IValidator<TValidation> validator)
            where T : class
            where TType : class, TValidation
            where TValidation : class
            => ruleBuilder.ChildRules(x => x.RuleFor(y => y.Value).SetValidator(validator));

        /// <summary>
        /// Checks that Optional HasValue is true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> MustHaveValue<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            where T : class
            where TProperty : IOptional
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((prop, context) =>
            {
                if (prop.HasValue)
                {

                }
                else
                {
                    context.AddFailure(context.PropertyPath, "API-ERROR.CORE.VALUE-MISSING");
                }
            });
        }

        /// <summary>
        /// Checks that Optional HasValue is true AND that the value is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, Optional<TType>> MustHaveValueNotNull<T, TType>(this IRuleBuilder<T, Optional<TType>> ruleBuilder)
            where T : class
        {
            return (IRuleBuilderOptions<T, Optional<TType>>)ruleBuilder.Custom((prop, context) =>
            {
                if (prop.HasValue)
                {
                    if(prop.Value == null)
                    {
                        context.AddFailure(context.PropertyPath, "API-ERROR.CORE.VALUE-NULL");
                    }
                }
                else
                {
                    context.AddFailure(context.PropertyPath, "API-ERROR.CORE.VALUE-MISSING");
                }
            });
        }

        /// <summary>
        /// Checks that Optional HasValue is true AND that the value is not null, empty, whitespace, or default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, Optional<TType>> MustHaveValueNotEmpty<T, TType>(this IRuleBuilder<T, Optional<TType>> ruleBuilder)
            where T : class
        {
            return (IRuleBuilderOptions<T, Optional<TType>>)ruleBuilder.Custom((prop, context) =>
            {
                if (prop.HasValue)
                {
                    if (prop.Value == null || prop.Value.Equals(default) || (prop.Value is string stringProp && string.IsNullOrWhiteSpace(stringProp)))
                    {
                        context.AddFailure(context.PropertyPath, "API-ERROR.CORE.VALUE-NULL");
                    }
                }
                else
                {
                    context.AddFailure(context.PropertyPath, "API-ERROR.CORE.VALUE-MISSING");
                }
            });
        }

        /// <summary>
        /// Checks that Optional HasValue is false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> MustNotHaveValue<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            where T : class
            where TProperty : IOptional
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((prop, context) =>
            {
                if (prop.HasValue)
                {
                    context.AddFailure(context.PropertyPath, "API-ERROR.CORE.VALUE-SHOULD-BE-EMPTY");
                }
                else
                {

                }
            });
        }

        /// <summary>
        /// Checks that all elements in a list are unique
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, List<TEntity>> UniqueElements<T, TEntity>(this IRuleBuilder<T, List<TEntity>> ruleBuilder)
            where T : class
            where TEntity : class
        {
            return (IRuleBuilderOptions<T, List<TEntity>>)ruleBuilder.Custom((prop, context) =>
            {
                var distincts = prop.Distinct();
                if(distincts.Count() != prop.Count)
                {
                    context.AddFailure(context.PropertyPath, "API-ERROR.CORE.DUPLICATED-VALUE-IN-LIST");
                }
            });
        }
    }
}
