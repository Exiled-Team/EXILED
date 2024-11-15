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

    using Exiled.API.Features.Core.Generic.Pools;

    /// <summary>
    /// A set of extensions for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly SHA256 Sha256 = SHA256.Create();

        /// <summary>
        /// Computes the distance between two <see cref="string"/>.
        /// </summary>
        /// <param name="source">The first string to be compared.</param>
        /// <param name="target">The second string to be compared.</param>
        /// <returns>The distance between the two strings.</returns>
        public static int LevenshteinDistance(this string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;

            if (string.IsNullOrEmpty(target))
                return source.Length;

            if (source.Length > target.Length)
                (source, target) = (target, source);

            int m = target.Length;
            int n = source.Length;
            int[,] distance = new int[2, m + 1];

            for (int j = 1; j <= m; j++)
                distance[0, j] = j;

            int currentRow = 0;
            for (int i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int previousRow = currentRow ^ 1;
                for (int j = 1; j <= m; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(
                        Math.Min(
                            distance[previousRow, j] + 1,
                            distance[currentRow, j - 1] + 1),
                        distance[previousRow, j - 1] + cost);
                }
            }

            return distance[currentRow, m];
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
        /// Converts a <see cref="string"/> to kebab case convention.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>A string converted to kebab case.</returns>
        public static string ToKebabCase(this string input) => Regex.Replace(input, "([a-z])([A-Z])", "$1_$2").ToLower();

        /// <summary>
        /// Converts a <see cref="string"/> from snake_case convention.
        /// </summary>
        /// <param name="str">The string to be converted.</param>
        /// <returns>Returns the new NotSnakeCase string.</returns>
        public static string FromSnakeCase(this string str)
        {
            string result = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '_')
                {
                    result += str[i + 1].ToString().ToUpper();
                    i++;
                }
                else
                {
                    result += str[i];
                }
            }

            return result;
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
            if (start > 0)
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

        /// <summary>
        /// Encrypts a value using SHA-256 and returns a hexadecimal hash string.
        /// </summary>
        /// <param name="value">The value to encrypt.</param>
        /// <returns>A hexadecimal hash string of the encrypted value.</returns>
        public static string GetHashedValue(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] hashBytes = Sha256.ComputeHash(bytes);

            StringBuilder hashStringBuilder = new();

            foreach (byte b in hashBytes)
                hashStringBuilder.Append(b.ToString("x2"));

            return hashStringBuilder.ToString();
        }
    }
}