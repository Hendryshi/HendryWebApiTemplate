using System.Text.RegularExpressions;

namespace Common.Application.Extensions
{
    public static class JsonExtensions
    {
        public static string RemoveJsonComments(this string jsonContent)
        {
            // Remove single-line comments (//)
            jsonContent = System.Text.RegularExpressions.Regex.Replace(jsonContent, @"//.*?$", "", RegexOptions.Multiline);

            // Remove multi-line comments (/* ... */)
            jsonContent = System.Text.RegularExpressions.Regex.Replace(jsonContent, @"/\*.*?\*/", "", RegexOptions.Singleline);

            return jsonContent;
        }
    }
}
