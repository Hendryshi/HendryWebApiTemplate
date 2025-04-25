namespace Common.Domain.Common
{
    /// <summary>
    /// Structure to use for command models on a nullable property
    /// Must not be set as nullable or it will cause problems with the serializer.
    ///     public Optional<int?> XXX is fine, 
    ///     public Optional<int>? XXX is NOT fine, 
    ///     public Optional<int> XXX is fine but NOT used in commands,
    /// The HasValue property indicates if a value (even 'null') was set. Will be false if created without a value.
    /// This makes the difference between null properties, and properties that were not given in the request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Optional<T> : IOptional<T>
    {
        private bool _hasValue;

        private T _value;

        public readonly bool HasValue => _hasValue;
        public readonly bool HasNoValue => !_hasValue;
        public readonly bool IsNotNull => HasValue && (Value != null);

        public readonly T Value
        {
            get
            {
                if (!_hasValue)
                {
                    return default;
                }

                return _value;
            }
        }

        public Optional(T value)
        {
            _hasValue = true;
            _value = value;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static implicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }

        public readonly object GetValue() => Value;
        public readonly Type GetValueType() => typeof(T);

        public void SetValue(object value)
        {
            _value = (T)value;
            _hasValue = true;
        }
    }

}
