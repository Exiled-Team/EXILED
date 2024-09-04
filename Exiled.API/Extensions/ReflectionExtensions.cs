// -----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Reflection;

    using HarmonyLib;

    using LiteNetLib.Utils;

    /// <summary>
    /// A set of extensions for <see cref="Type"/>.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Invokes a static method.
        /// </summary>
        /// <param name="type">The method type.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="param">The method parameters.</param>
        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;

            type.GetMethod(methodName, flags)?.Invoke(null, param);
        }

        /// <summary>
        /// Invokes a static event.
        /// </summary>
        /// <param name="type">The event type.</param>
        /// <param name="eventName">The event name.</param>
        /// <param name="param">The event arguments.</param>
        public static void InvokeStaticEvent(this Type type, string eventName, object[] param)
        {
            MulticastDelegate eventDelegate = (MulticastDelegate)type.GetField(eventName, AccessTools.all).GetValue(null);
            if (eventDelegate != null)
            {
                foreach (Delegate handler in eventDelegate.GetInvocationList())
                {
                    handler.Method.Invoke(handler.Target, param);
                }
            }
        }

        /// <summary>
        /// Copy all properties from the source class to the target one.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="source">The source object to copy properties from.</param>
        public static void CopyProperties(this object target, object source)
        {
            Type type = target.GetType();

            if (type != source.GetType())
                throw new InvalidTypeException("Target and source type mismatch!");

            foreach (PropertyInfo sourceProperty in type.GetProperties())
                type.GetProperty(sourceProperty.Name)?.SetValue(target, sourceProperty.GetValue(source, null), null);
        }

        /// <summary>
        /// Removes the generic type suffix (e.g., '`1' from `List`1`) from a type name if it exists.
        /// </summary>
        /// <param name="typeName">The name of the type, which may include a generic suffix.</param>
        /// <returns>The type name without the generic suffix if it was present; otherwise, returns the original type name.</returns>
        public static string RemoveGenericSuffix(this string typeName)
        {
            int indexOfBacktick = typeName.IndexOf('`');
            return indexOfBacktick >= 0 ? typeName.Substring(0, indexOfBacktick) : typeName;
        }

        /// <summary>
        /// Gets the first non-generic base type of a given type.
        /// </summary>
        /// <param name="type">The type for which to find the first non-generic base type.</param>
        /// <returns>The first non-generic base type, or null if none is found.</returns>
        public static Type GetFirstNonGenericBaseType(this Type type)
        {
            Type baseType = type.BaseType;

            while (baseType != null && baseType.IsGenericType)
                baseType = baseType.BaseType;

            return baseType;
        }
    }
}