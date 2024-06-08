// -----------------------------------------------------------------------
// <copyright file="BitwiseExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Extensions for bitwise operations.
    /// </summary>
    public static class BitwiseExtensions
    {
        /// <summary>
        /// Sets the specified flag to the given value, default is true.
        /// </summary>
        /// <param name="flags">The flags enum to modify.</param>
        /// <param name="flag">The flag to set.</param>
        /// <param name="value">The value to set the flag to.</param>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <returns>The flags enum with the flag set to the given value.</returns>
        public static T SetFlag<T>(this T flags, T flag, bool value = true)
            where T : Enum
        {
            long flagsValue = Convert.ToInt64(flags);
            long flagValue = Convert.ToInt64(flag);

            return (T)Enum.ToObject(typeof(T), value
                ? flagsValue | flagValue
                : flagsValue & ~flagValue);
        }
    }
}