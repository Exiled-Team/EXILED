// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extensions for Enums.
    /// </summary>
    public static class EnumExtensions
    {
        private static Dictionary<Type, IEnumerable<object>> storedEnumValues = new();

        /// <summary>
        /// Queries an enum and returns their values.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>An IEnumerable{T} of the enum values from the provided enum.</returns>
        public static IEnumerable<T> QueryEnumValue<T>()
            where T : Enum
        {
            Type type = typeof(T);

            if (!storedEnumValues.ContainsKey(type))
            {
                storedEnumValues.Add(type, Enum.GetValues(type).ToArray<object>());
            }

            return storedEnumValues[type].Cast<T>();
        }
    }
}