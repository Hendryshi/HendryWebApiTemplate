using FluentResults;

namespace Common.Application.Errors
{
    public class InvalidStatusError : Error
    {
        public InvalidStatusError(string message) : base(message) { }
        public InvalidStatusError(string message, Error causeBy) : base(message, causeBy) { }
    }
}
