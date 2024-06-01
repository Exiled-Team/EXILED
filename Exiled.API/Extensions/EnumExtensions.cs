// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    /// <summary>
    /// Extensions for Enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Queries an enum and returns their values.
        /// </summary>
        /// <typeparam name="T">The type of enum to query.</typeparam>
        /// <returns>An array of the enum values from the provided enum.</returns>
        public static T[] QueryValues<T>()
            where T : struct, Enum
            => EnumUtils<T>.Values;

        /// <summary>
        /// Queries an enum and returns their names.
        /// </summary>
        /// <typeparam name="T">The type of enum to query.</typeparam>
        /// <returns>An array of the enum names from the provided enum.</returns>
        public static string[] QueryNames<T>()
            where T : struct, Enum
            => EnumUtils<T>.Names;

        /// <summary>
        /// Queries an enum and returns its underlying type.
        /// </summary>
        /// <typeparam name="T">The type of enum to query.</typeparam>
        /// <returns>The underlying type of the enum.</returns>
        public static Type QueryUnderlyingType<T>()
            where T : struct, Enum
            => EnumUtils<T>.UnderlyingType;

        /// <summary>
        /// Queries an enum and returns its underlying type code.
        /// </summary>
        /// <typeparam name="T">The type of enum to query.</typeparam>
        /// <returns>The underlying type code of the enum.</returns>
        public static TypeCode QueryTypeCode<T>()
            where T : struct, Enum
            => EnumUtils<T>.TypeCode;
    }
}