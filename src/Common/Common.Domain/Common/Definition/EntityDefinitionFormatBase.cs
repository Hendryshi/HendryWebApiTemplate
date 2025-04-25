namespace Common.Domain.Common.Definition
{
    public abstract class EntityDefinitionFormatBase : Enumeration
    {
        public string FormatValue { get; set; }

        public List<Data> AdditionalData { get; set; }

        protected EntityDefinitionFormatBase() : base() { }

        protected EntityDefinitionFormatBase(string value, string displayName, string formatValue, List<Data> additionalData = null) : base(value, displayName)
        {
            FormatValue = formatValue;
            AdditionalData = additionalData;
        }

        public class Data
        {
            public string Key;
            public string Value;
            public bool IsLocalized;

            public Data(string key, string value, bool isLocalized = false)
            {
                Key = key;
                Value = value;
                IsLocalized = isLocalized;
            }
        }
    }
}
