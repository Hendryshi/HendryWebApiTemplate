namespace Common.Domain.Common
{
    /// <summary>
    /// used only for generic types limitations
    /// DO NOT USE TO INDICATE OPTIONAL FIELDS IN REQUESTS! Use the Optional<> structure instead
    /// </summary>
    public interface IOptional<T> : IOptional
    {
        T Value { get; }
    }

    /// <summary>
    /// used only for generic types limitations
    /// DO NOT USE TO INDICATE OPTIONAL FIELDS IN REQUESTS! Use the Optional<> structure instead
    /// </summary>
    public interface IOptional
    {
        bool HasValue { get; }
        bool HasNoValue { get; }
        bool IsNotNull { get; }
        object GetValue();
        Type GetValueType();
        void SetValue(object value);
    }
}
