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

    using LiteNetLib.Utils;

    using YamlDotNet.Serialization;

    /// <summary>
    /// A set of extensions for <see cref="Type"/>.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Invoke a static method.
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
            {
                if (type.GetProperty(sourceProperty.Name) is { } property && property.GetCustomAttribute<YamlIgnoreAttribute>() is null && property.SetMethod is not null)
                    property?.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
        }
    }
}