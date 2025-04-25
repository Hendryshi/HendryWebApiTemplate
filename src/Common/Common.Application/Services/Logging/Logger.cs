using Microsoft.Extensions.Logging;

namespace Common.Application.Services.Logging
{
    public static class Logger
    {

        private static ILoggerFactory _Factory = null;

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                _Factory ??= new LoggerFactory();
                return _Factory;
            }
            set { _Factory = value; }
        }

        /// <summary>
        /// Create a logger using the name of the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        /// <summary>
        /// Create a logger using the category name
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }

}
