﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMap.Extensions
{
    /// <summary>
    /// type extensions
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Returns true if type if system type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSystemType(this Type type) => type.Namespace != null && type.Namespace.StartsWith("System");

        /// <summary>
        /// Returns true if type is IEnumerable type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIEnumerableType(this Type type) => type.Namespace != null && type.Namespace.StartsWith("System.Collections");

        /// <summary>
        /// Instantiates an object given dynamically defined type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Instantiate(this Type type) => Activator.CreateInstance(type);

        /// <summary>
        /// Gets T from IEnumerable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetGenericType(this Type type)
        {
            // type is Array
            // short-circuit if you expect lots of arrays 
            if (type.IsArray)
                return type.GetElementType();

            // type is IEnumerable<T>;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // type implements/extends IEnumerable<T>;
            var enumType = type.GetInterfaces()
                .Where(t => t.IsGenericType &&
                            t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
            return enumType ?? type;
        }
        
        /// <summary>
        /// Checks whether if type is pritmive of system type, essentially more advanced version of IsPrimitive
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitiveSystemType(this Type type) => type.IsPrimitive || type.IsValueType || type == typeof(string)
                                                                    || type.IsIEnumerableType() && type.GetGenericType().IsPrimitiveSystemType();

        /// <summary>
        /// Checks whether if type is dictionary or not
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool IsDictionaryType(this Type type, out Type keyType, out Type valueType)
        {
            var flag = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);

            var args = type.GetGenericArguments();
            keyType = args.FirstOrDefault();
            valueType = args.LastOrDefault();

            return flag;
        }
    }
}