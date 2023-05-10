﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    ///
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///
        /// </summary>
        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
        /// <summary>
        ///
        /// </summary>
        public static bool IsAssignableFrom(this Type type, Type other)
        {
            if (other==null)
            {
                return false;
            }
            return type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
        }
        /// <summary>
        ///
        /// </summary>
        public static bool IsPrimitive(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }
        /// <summary>
        ///
        /// </summary>
        public static bool IsSubclassOf(this Type type, Type other)
        {
            return type.GetTypeInfo().IsSubclassOf(other);
        }
        /// <summary>
        ///
        /// </summary>
        public static MethodInfo GetMethod(this Type type, string propertyName)
        {
            return type.GetTypeInfo().GetDeclaredMethod(propertyName);
        }
        /// <summary>
        ///
        /// </summary>
        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            return type.GetRuntimeProperty(propertyName);
        }
        /// <summary>
        ///
        /// </summary>
        public static PropertyInfo[] GetPublicInstanceProperties(this Type type)
        {
            var result = type.GetRuntimeProperties().ToArray();
            return result;
        }
        /// <summary>
        ///
        /// </summary>
        public static Type BaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }
        /// <summary>
        ///
        /// </summary>
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }
        /// <summary>
        ///
        /// </summary>
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GenericTypeArguments;
        }

    }
}