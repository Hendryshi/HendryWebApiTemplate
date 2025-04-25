using Common.Domain.Serialization;
using FluentResults;
using Newtonsoft.Json;
using System.Reflection;

namespace Common.Domain.Common.Definition
{
    /// <summary>
    /// https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    /// </summary>
    [JsonConverter(typeof(EnumerationConverter))]
    ///Do not remove the converter it allows the enum to be serialized as string value.
    ///If serliazed as full object we have issue with deserilization  when domain entity have default value with one enum value. 
    ///In that case, the enum reference value is changed with serialized data for the whole application !!!!!!!!!
    public abstract class Enumeration : IComparable
    {
        private string _value;
        private string _displayName;

        protected Enumeration()
        { }

        protected Enumeration(string value, string displayName)
        {
            _value = value;
            _displayName = displayName;
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }


        public override string ToString()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = _value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        //public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        //{
        //    var absoluteDifference = Math.Abs(firstValue.Value - secondValue.Value);
        //    return absoluteDifference;
        //}

        public int CompareTo(object other)
        {
            if (other == null) throw new ArgumentException("Cannot compare to a null value");
            return Value.CompareTo(((Enumeration)other).Value);
        }

        /// <summary>
        /// Retrieve the item from its string value.
        /// Raise an exception if not found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromValue<T>(string value) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(value, "value", item => item.Value.Equals(value));
            return matchingItem;
        }

        public static Result<T> TryFromValue<T>(string value) where T : Enumeration
        {
            T matchingItem;
            try
            {
                matchingItem = FromValue<T>(value);
            }
            catch (Exception e)
            {
                return Result.Fail(new Error(e.Message).CausedBy(e));
            }
            return Result.Ok(matchingItem);
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.DisplayName.Equals(displayName));
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = string.Format(/*Localizer.GetUIMessage(*/"Ptolemy.Enumeration.BadRequest.ValueNotValid",
                    new { Value = value, Description = description, typeof(T).Name });
                throw new InvalidOperationException(message);
            }

            return matchingItem;
        }

        /// <summary>
        /// Get all items of the enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(f => (T)f.GetValue(null));

            //foreach (var info in fields)
            //{
            //    var instance = new T();
            //    var locatedValue = info.GetValue(instance) as T;

            //    if (locatedValue != null)
            //    {
            //        yield return locatedValue;
            //    }
            //}
        }

        /// <summary>
        /// Get all items of the enumeration matching the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>(Func<T, bool> predicate = null) where T : Enumeration
        {
            predicate ??= x => true;
            return GetAll<T>().Where(predicate);
        }

        /// <summary>
        /// Get all item values of the enumeration matching the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllValues<T>(Func<T, bool> predicate = null) where T : Enumeration
        {
            predicate ??= x => true;
            return GetAllItems(predicate).Select(x => x.Value);
        }

        /// <summary>
        /// Get all items of the enumeration matching the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> GetAllItemsList<T>(Func<T, bool> predicate = null) where T : Enumeration
        {
            return GetAllItems(predicate).ToList();
        }

        /// <summary>
        /// Get all item values of the enumeration matching the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<string> GetAllValuesList<T>(Func<T, bool> predicate = null) where T : Enumeration
        {
            return GetAllValues(predicate).ToList();
        }
    }

    public static class EnumerationExtensions
    {
        public static List<string> ToListDisplayNameLocalized<T>(this List<T> list, string culture = "", bool useDefaultCulture = false) where T : Enumeration
        {
            return list.Select(x => x.DisplayName).ToList();
        }
    }


}
