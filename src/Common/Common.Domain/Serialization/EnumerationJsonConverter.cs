using Common.Domain.Common.Definition;
using Newtonsoft.Json;

namespace Common.Domain.Serialization
{
    /// <summary>
    /// This converter is used to write enumeration as string (its value) in the json.
    /// The converter will not read the json, but the serialized string will be converted automatically to the enumeration value  using the implicit operator defined in each derived class.
    /// </summary>
    public class EnumerationConverter : JsonConverter
    {
        /// <summary>
        /// Write the enumeration as string, its value
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) throw new ArgumentNullException("json file in given in WriteJson method cannot be null");
            Enumeration @enum = (Enumeration)value;
            writer.WriteValue(@enum.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("No read of Json for Enumeration we want the implicit operator to convert string to type");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enumeration);
        }

        /// <summary>
        /// Block the converter to execute its ReadJson()
        /// </summary>
        public override bool CanRead => false;
    }
}
