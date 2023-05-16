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
    /// Contains an extension method to <see cref="System.Enum"/>.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converting an Enum to a Bool Array.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <param name="enum">The enum to convert.</param>
        /// <returns>A Array of <see cref="bool"/> determining the value.</returns>
        public static bool[] ToBoolArray<T>(this T @enum)
            where T : Enum
        {
            int[] values = (int[])Enum.GetValues(@enum.GetType());
            bool[] arr = new bool[values.Length + 1];

            for (var i = 0; i < arr.Length; i++)
                arr[i] = @enum.HasFlag((T)(object)(1 << i));

            return arr;
        }

        /// <summary>
        /// Converting a Bool Array to an Enum.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <param name="arr">The array of bool to convert.</param>
        /// <returns>The <typeparamref name="T"/> module that was requested.</returns>
        public static T ToFlagEnum<T>(this bool[] arr)
            where T : Enum
        {
            if (typeof(T).BaseType != typeof(Enum))
                throw new TypeAccessException("This extension method is for flag enums");

            T @enum = default;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i])
                    @enum = (T)(object)((int)(object)@enum | (1 << i));
            }

            return @enum;
        }
    }
}
