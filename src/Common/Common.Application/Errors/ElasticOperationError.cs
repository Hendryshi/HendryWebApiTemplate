using FluentResults;

namespace Common.Application.Errors
{
    public class ElasticOperationError : Error
    {
        public ElasticOperationError(string message) : base(message) { }
        public ElasticOperationError(string message, Error causeBy) : base(message, causeBy) { }
    }
}
