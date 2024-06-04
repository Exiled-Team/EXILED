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
    /// Extensions for Bitwise operations.
    /// </summary>
    public static class BitwiseExtensions
    {
        /// <summary>
        /// Sets or clears the specified flag from the flags.
        /// </summary>
        /// <param name="flags">Original flags value.</param>
        /// <param name="flag">Flag to change.</param>
        /// <param name="value">if true will add, else clears.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The modified value with the flag cleared/set.</returns>
        /// <remarks>Originally from Michal78900/MapEditorReborn repo on Github.</remarks>
        public static T SetFlag<T>(this T flags, T flag, bool value)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            int flagsInt = flags.ToInt32(NumberFormatInfo.CurrentInfo);
            int flagInt = flag.ToInt32(NumberFormatInfo.CurrentInfo);
            if (value)
            {
                flagsInt |= flagInt;
            }
            else
            {
                flagsInt &= ~flagInt;
            }

            return (T)(object)flagsInt;
        }
    }
}