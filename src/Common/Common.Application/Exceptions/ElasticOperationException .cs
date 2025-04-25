namespace Common.Application.Exceptions
{
    public class ElasticOperationException : Exception
    {
        public ElasticOperationException() : base()
        {
        }
        public ElasticOperationException(string message) : base(message)
        {
        }

        public ElasticOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
