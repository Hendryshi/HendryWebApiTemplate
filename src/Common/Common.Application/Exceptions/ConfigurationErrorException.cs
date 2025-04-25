namespace Common.Application.Exceptions
{
    internal class ConfigErrorException : Exception
    {
        public ConfigErrorException()
           : base()
        {
        }

        public ConfigErrorException(string message)
            : base(message)
        {
        }

        public ConfigErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
