// -----------------------------------------------------------------------
// <copyright file="BitwiseExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    /// <summary>
    /// Extensions for bitwise operations.
    /// </summary>
    public static class BitwiseExtensions
    {
        /// <summary>
        /// Adds the specified flags to the given enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="flags">The enum value to add flags to.</param>
        /// <param name="newFlags">The flags to add.</param>
        /// <returns>The enum value with the specified flags added.</returns>
        public static T AddFlags<T>(this T flags, params T[] newFlags)
            where T : Enum => flags.ModifyFlags(true, newFlags);

        /// <summary>
        /// Removes the specified flags from the given enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="flags">The enum value to remove flags from.</param>
        /// <param name="oldFlags">The flags to remove.</param>
        /// <returns>The enum value with the specified flags removed.</returns>
        public static T RemoveFlags<T>(this T flags, params T[] oldFlags)
            where T : Enum => flags.ModifyFlags(false, oldFlags);

        /// <summary>
        /// Sets the specified flag to the given value, default is true.
        /// </summary>
        /// <param name="flags">The flags enum to modify.</param>
        /// <param name="value">The value to set the flag to.</param>
        /// <param name="changeFlags">The flags to modify.</param>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <returns>The flags enum with the flag set to the given value.</returns>
        public static T ModifyFlags<T>(this T flags, bool value, params T[] changeFlags)
            where T : Enum
        {
            long currentValue = Convert.ToInt64(flags);

            foreach (T flag in changeFlags)
            {
                long flagValue = Convert.ToInt64(flag);

                if (value)
                    currentValue |= flagValue;
                else
                    currentValue &= ~flagValue;
            }

            return (T)Enum.ToObject(typeof(T), currentValue);
        }
    }
}