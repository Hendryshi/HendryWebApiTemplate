namespace Common.Application.Exceptions
{
    public class InvalidStatusException : Exception
    {
        public InvalidStatusException() : base()
        {
        }
        public InvalidStatusException(string message) : base(message)
        {
        }

        public InvalidStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
