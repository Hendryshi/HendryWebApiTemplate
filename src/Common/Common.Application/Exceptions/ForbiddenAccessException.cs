namespace Common.Application.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        /// <summary>
        /// Exception for forbidden access to ressource
        /// </summary>
        public ForbiddenAccessException()
            : base()
        {
        }

        /// <summary>
        /// Exception for forbidden access to ressource with message
        /// </summary>
        /// <param name="message"></param>
        public ForbiddenAccessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Exception for forbidden access to ressource with message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ForbiddenAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
