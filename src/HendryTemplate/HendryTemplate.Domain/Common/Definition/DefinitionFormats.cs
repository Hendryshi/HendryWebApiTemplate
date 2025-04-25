using Common.Domain.Common.Definition;

namespace HendryTemplate.Domain.Common.Definition
{
    public class DefinitionFormats : EntityDefinitionFormatBase
    {
        public static readonly Dictionary<string, Type> TableNames = new()
        {
            //{ Formats.PaymentMode           , typeof(DEFPaymentMode)            }
        };

        public static class Formats
        {
            //Common
            //public const string PaymentMode = Payment_FORMATValues.PaymentMode;

            public static List<String> ToList()
            {
                return typeof(Formats).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Select(t => t.GetValue(null).ToString())
                    .ToList<String>();
            }
        }

        protected DefinitionFormats() : base() { }
    }
}
