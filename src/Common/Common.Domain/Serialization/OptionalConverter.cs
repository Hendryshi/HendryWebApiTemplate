using Common.Domain.Common;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Domain.Serialization
{
    public class OptionalConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(Optional<>))
            {
                return false;
            }

            return true;
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            Type entityType = type.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(OptionalConverterInner<>).MakeGenericType(
                    new Type[] { entityType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null)!;

            return converter;
        }

        private class OptionalConverterInner<T> :
            JsonConverter<Optional<T>>
        {
            private readonly JsonConverter<T> _valueConverter;
            private readonly Type _entityType;

            public OptionalConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter.
                _valueConverter = (JsonConverter<T>)options
                    .GetConverter(typeof(T));

                _entityType = typeof(T);
            }

            public override Optional<T> Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return new Optional<T>(default);
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    var valueJson = reader.GetString();

                    Optional<T> optional = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(valueJson);
                    return optional;
                }
                else
                {
                    using var jsonDocument = JsonDocument.ParseValue(ref reader);
                    var valueJson = jsonDocument.RootElement.GetRawText();

                    Optional<T> optional = JsonSerializer.Deserialize<T>(valueJson, options);
                    return optional;
                }
            }

            public override void Write(
                Utf8JsonWriter writer,
                Optional<T> optional,
                JsonSerializerOptions options)
            {
                if (optional.HasValue)
                {
                    _valueConverter.Write(writer, optional.Value, options);
                }
                else
                {
                    //writer.WriteNull(writer.proper);
                }
            }
        }
    }
}
