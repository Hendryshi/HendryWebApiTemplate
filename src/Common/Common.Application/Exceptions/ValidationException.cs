using FluentValidation.Results;

namespace Common.Application.Exceptions
{
    public class ValidationException : Exception
    {
        /// <summary>
        /// Exception for validation failures
        /// </summary>
        public ValidationException()
            : base()
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Exception for validation failures with message
        /// </summary>
        /// <param name="message"></param>
        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Exception for validation failures with message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this("One or more validation failures have occurred.")
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
