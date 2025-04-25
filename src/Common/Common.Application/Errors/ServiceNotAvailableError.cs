using FluentResults;

namespace Common.Application.Errors
{
    /// <summary>
    /// Must be used to return status code 500 (Bad Request) 
    /// This should be used to indicate API malfunction 500 is the generic REST API error response.
    /// 5xx codes tell the client something happened on the server and their request by itself was perfectly valid. The client can continue and try again with the request without modification.
    /// </summary>
    public class ServiceNotAvailableError : Error
    {
        public ServiceNotAvailableError(string message) : base(message) { }
        public ServiceNotAvailableError(string message, Error causeBy) : base(message, causeBy) { }
    }
}
