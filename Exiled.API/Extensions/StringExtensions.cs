// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    using Exiled.API.Features.Pools;

    /// <summary>
    /// A set of extensions for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly SHA256 Sha256 = SHA256.Create();

        /// <summary>
        /// Compute the distance between two <see cref="string"/>.
        /// </summary>
        /// <param name="firstString">The first string to be compared.</param>
        /// <param name="secondString">The second string to be compared.</param>
        /// <returns>Returns the distance between the two strings.</returns>
        public static int GetDistance(this string firstString, string secondString)
        {
            int n = firstString.Length;
            int m = secondString.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = secondString[j - 1] == firstString[i - 1] ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        /// <summary>
        /// Extract command name and arguments from a <see cref="string"/>.
        /// </summary>
        /// <param name="commandLine">The <see cref="string"/> to extract from.</param>
        /// <returns>Returns a <see cref="ValueTuple"/> containing the exctracted command name and arguments.</returns>
        public static (string commandName, string[] arguments) ExtractCommand(this string commandLine)
        {
            string[] extractedArguments = commandLine.Split(' ');

            return (extractedArguments[0].ToLower(), extractedArguments.Skip(1).ToArray());
        }

        /// <summary>
        /// Converts a <see cref="string"/> to snake_case convention.
        /// </summary>
        /// <param name="str">The string to be converted.</param>
        /// <param name="shouldReplaceSpecialChars">Indicates whether special chars has to be replaced or not.</param>
        /// <returns>Returns the new snake_case string.</returns>
        public static string ToSnakeCase(this string str, bool shouldReplaceSpecialChars = true)
        {
            string snakeCaseString = string.Concat(str.Select((ch, i) => (i > 0) && char.IsUpper(ch) ? "_" + ch.ToString() : ch.ToString())).ToLower();

            return shouldReplaceSpecialChars ? Regex.Replace(snakeCaseString, @"[^0-9a-zA-Z_]+", string.Empty) : snakeCaseString;
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> into a string.
        /// </summary>
        /// <typeparam name="T">The type of the IEnumerable.</typeparam>
        /// <param name="enumerable">The instance.</param>
        /// <param name="showIndex">Indicates whether the enumerator index should be shown or not.</param>
        /// <returns>Returns the converted <see cref="IEnumerable{T}"/>.</returns>
        public static string ToString<T>(this IEnumerable<T> enumerable, bool showIndex = true)
        {
            StringBuilder stringBuilder = StringBuilderPool.Pool.Get();
            int index = 0;

            stringBuilder.AppendLine();

            foreach (T enumerator in enumerable)
            {
                if (showIndex)
                    stringBuilder.Append($"{index++} ");

                stringBuilder.AppendLine(enumerator.ToString());
            }

            return StringBuilderPool.Pool.ToStringReturn(stringBuilder);
        }

        /// <summary>
        /// Removes the prefab-generated brackets (#) on <see cref="UnityEngine.GameObject"/> names.
        /// </summary>
        /// <param name="name">Name of the <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>Name without brackets.</returns>
        public static string RemoveBracketsOnEndOfName(this string name)
        {
            int bracketStart = name.IndexOf('(') - 1;

            if (bracketStart > 0)
                name = name.Remove(bracketStart, name.Length - bracketStart);

            return name;
        }

        /// <summary>
        /// Retrieves a string before a symbol from an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns>Substring before the symbol.</returns>
        public static string GetBefore(this string input, char symbol)
        {
            int start = input.IndexOf(symbol);
            if (start != 0)
                input = input.Substring(0, start);

            return input;
        }

        /// <summary>
        /// Splits camel case string to space-separated words. Ex: SomeCamelCase -> Some Camel Case.
        /// </summary>
        /// <param name="input">Camel case string.</param>
        /// <returns>Splitted string.</returns>
        public static string SplitCamelCase(this string input) => Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();

        /// <summary>
        /// Removes all space symbols from string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>String without spaces.</returns>
        public static string RemoveSpaces(this string input) => Regex.Replace(input, @"\s+", string.Empty);

        /// <summary>
        /// Gets the player's user id without the authentication.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns the raw user id.</returns>
        public static string GetRawUserId(this string userId) => userId.Substring(0, userId.LastIndexOf('@'));

        /// <summary>
        /// Gets a SHA256 hash of a player's user id without the authentication.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The hashed userid.</returns>
        public static string GetHashedUserId(this string userId)
        {
            
            byte[] textData = Encoding.UTF8.GetBytes(userId.Substring(0, userId.LastIndexOf('@')));
            byte[] hash = Sha256.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}