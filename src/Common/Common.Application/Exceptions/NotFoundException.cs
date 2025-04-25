namespace Common.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Exception for ressource not found
        /// </summary>
        public NotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Exception for ressource not found with message
        /// </summary>
        /// <param name="message"></param>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Exception for ressource not found with message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}
