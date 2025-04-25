using FluentResults;

namespace Common.Application.Errors
{
    public class ObjectAlreadyExistsError : Error
    {
        public ObjectAlreadyExistsError(string message) : base(message) { }
        public ObjectAlreadyExistsError(string message, Error causeBy) : base(message, causeBy) { }
    }
}
