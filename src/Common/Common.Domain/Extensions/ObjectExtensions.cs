using Newtonsoft.Json;

namespace Common.Domain.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Perform a deep copy of the object via JSON serialization.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>A deep copy of the object.</returns>
        public static T Clone<T>(this T source) where T : class
        {
            // Don't serialize a null object, simply return the default for that object
            if (source is null) return null;

            var file = JsonConvert.SerializeObject(source);
            var copy = (T)JsonConvert.DeserializeObject(file, source.GetType());

            return copy;
        }

        public static string TrimStackTrace(this string stackTrace)
        {
            return string.Join("\n", stackTrace.Split("\n").Where(x => !x.ToString().Contains("System.") && !x.ToString().Contains("Microsoft.") && !x.ToString().Contains("Npgsql.") && !x.ToString().Contains("MediatR.")));
        }

    }
}
