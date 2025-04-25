namespace Common.Application.Exceptions
{
    public class ObjectAlreadyExistsException : Exception
    {
        public ObjectAlreadyExistsException()
           : base()
        {
        }
        public ObjectAlreadyExistsException(string message) : base(message)
        {
        }

        public ObjectAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
