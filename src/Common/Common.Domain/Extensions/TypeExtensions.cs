namespace Common.Domain.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gives all of the BaseTypes the current types inherits from
        /// Does not give interfaces
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Type> GetInheritedTypes(this Type type)
        {
            var types = new List<Type>() { type };
            Type currentType = type;
            while (currentType.BaseType != null)
            {
                types.Add(currentType.BaseType);
                currentType = currentType.BaseType;
            }

            return types;
        }

        /// <summary>
        /// Tells if the current type is a generic list
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGenericList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
