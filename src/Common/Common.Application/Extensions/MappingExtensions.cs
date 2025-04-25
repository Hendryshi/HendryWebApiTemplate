using AutoMapper;
using Common.Application.Commands;
using Common.Application.Exceptions;
using Common.Domain.Common;
using Common.Domain.Extensions;
using System.Collections;

namespace Common.Application.Extensions
{
    public static class MappingExtensions
    {
        #region Application commands > Domain
        /// <summary>
        /// Adds basic mapping rules to map a BaseEntityCommand object with Optional values to a BaseEntity object.
        /// Ignores all non Optional values and all Optional values with no defined Value property.
        /// Gives the logic of how to map Optional values.
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static IMappingExpression<TCommand, TEntity> OptionalValueRules<TCommand, TEntity>(this IMappingExpression<TCommand, TEntity> mapper)
            where TCommand : class, new()
            where TEntity : class, new()
        {
            mapper.IgnoreAllPropertiesWithAnInaccessibleSetter();
            mapper.ForAllMembers(opt =>
            {
                opt.PreCondition((src, context) => !src.GetType().GetProperty(opt.DestinationMember.Name).PropertyType.IsAssignableTo(typeof(IOptional)));
            });

            mapper.IgnoreAllPropertiesWithAnInaccessibleSetter();
            mapper.AfterMap((src, dest, context) =>
            {
                // treat all optional values. Apparently, the MapFrom causes all kinds of problems
                foreach(var prop in src.GetType().GetProperties().Where(x => x.PropertyType.IsAssignableTo(typeof(IOptional))))
                {
                    var destProp = dest.GetType().GetProperty(prop.Name);
                    if(destProp == null || destProp.SetMethod == null || (destProp.SetMethod != null && !destProp.SetMethod.IsPublic)) continue;
                    var value = GetOptionalValue(prop.Name, src, dest, context);
                    if(value != null && destProp.PropertyType.IsGenericList() && destProp.PropertyType.GenericTypeArguments[0].IsAssignableTo(typeof(BaseEntity)))
                    { // if list of entities, the value must be treated for assignment. Cast problems otherwise.
                        var genType = destProp.PropertyType.GenericTypeArguments[0];
                        var targetType = typeof(List<>).MakeGenericType(genType);
                        IList newList = (IList)Activator.CreateInstance(targetType); // new list of the right type

                        foreach(var element in value as IList) newList.Add(element); // transfer the elements into the new list
                        destProp.SetValue(dest, newList);
                    }
                    else destProp.SetValue(dest, value);
                }
            });

            return mapper;
        }

        private static object GetOptionalValue<TCommand, TEntity>(string propName, TCommand src, TEntity dest, ResolutionContext context)
            where TCommand : class, new()
            where TEntity : class, new()
        {
            var srcOptional = src.GetType().GetProperty(propName).GetValue(src, null) as IOptional;
            var destProperty = dest.GetType().GetProperty(propName);
            var destValue = destProperty.GetValue(dest, null);

            if(srcOptional.HasNoValue) return destValue;

            var srcValue = srcOptional.GetValue();

            // no need to go further
            if(srcValue == null) return null;

            // check that src is a list
            if(!srcValue.GetType().IsGenericList())
            {
                // if not, just map the value directly
                // Except in the case of datetime, where we want to make sure it is switched to Utc (for the Database)
                //if (destProperty.PropertyType == typeof(DateTime) || destProperty.PropertyType == typeof(DateTime?))
                //{
                //    return ((DateTime)srcValue).ToUniversalTime();
                //}

                if(destValue != default)
                    return context.Mapper.Map(srcValue, destValue, srcValue.GetType(), destProperty.PropertyType);
                else
                    return context.Mapper.Map(srcValue, srcValue.GetType(), destProperty.PropertyType);
            }

            /* --------------- if src Value is list ---------------- */

            // first check if dest property is a string. if so, join on common separator
            if(destProperty.PropertyType == typeof(string)) return string.Join(';', srcValue as IEnumerable);

            // then check if dest list is list of Base Entities
            if(destProperty.PropertyType.GenericTypeArguments.Length > 0)
            {
                if(destProperty.PropertyType.GenericTypeArguments.Any(t => t.IsSubclassOf(typeof(BaseEntity))))
                {
                    // if so, src list must be a list of commands
                    var listCommands = (srcValue as IEnumerable).Cast<BaseEntityCommand>();

                    // init the return list
                    IList destList = new List<BaseEntity>();

                    // get the current destination list if any exists (like during an update)
                    var currentDestList = (destValue as IEnumerable)?.Cast<BaseEntity>();
                    currentDestList ??= [];

                    // get the source and destination generic list types
                    var srcElementType = srcValue.GetType().GetGenericArguments()[0];
                    var destElementType = destProperty.PropertyType.GetGenericArguments()[0];

                    foreach(var command in listCommands)
                    {
                        if(Guid.TryParse(command.Id, out Guid gId) && gId != Guid.Empty) // check all elements to update
                        {
                            try
                            {
                                var linked = currentDestList.Single(x => x.Id.ToString() == command.Id); // recover the corresponding entity in dest list. Exception on failure
                                destList.Add((BaseEntity)context.Mapper.Map(command, linked, srcElementType, destElementType));
                            }
                            catch
                            {
                                // failure to get matching element. Id should not have been given
                                throw new NotFoundException($"Failed to recover a {destElementType.Name} of id {command.Id} from {propName} in destination entity {dest.GetType().Name}");
                            }
                        }
                        else
                        { // elements to create
                            destList.Add((BaseEntity)context.Mapper.Map(command, srcElementType, destElementType));
                        }
                    }

                    return destList;
                }
                else
                {
                    if(destValue is null)
                    {
                        var srcElementType = srcValue.GetType();
                        var destElementType = destProperty.PropertyType;
                        return context.Mapper.Map(srcValue, srcElementType, destElementType);
                    }
                    else
                        return context.Mapper.Map(srcValue, destValue);
                }
            }
            else
            {
                // both src and dest lists must be lists of common types, like int or string. we can do a direct replacement
                return srcValue;
            }
        }
        #endregion
    }
}
