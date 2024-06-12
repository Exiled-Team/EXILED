// -----------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HarmonyLib;

    /// <summary>
    /// A set of extensions for easily interact with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes elements from the enumerable that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to remove elements from.</param>
        /// <param name="func">The condition used to determine which elements to remove.</param>
        /// <returns>A new enumerable with elements removed based on the specified condition.</returns>
        public static IEnumerable<T> RemoveSpecified<T>(this IEnumerable<T> enumerable, Func<T, bool> func)
        {
            foreach (T item in enumerable)
            {
                if (!func(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets a random item from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to get the item from.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <returns>A random item from the <see cref="IEnumerable{T}"/>.</returns>
        public static T Random<T>(this IEnumerable<T> enumerable) =>
            (enumerable as T[] ?? enumerable.ToArray()) is { Length: > 0 } arr ? arr[UnityEngine.Random.Range(0, arr.Length)] : default;

        /// <summary>
        /// Gets a random item from an <see cref="IEnumerable{T}"/> given a condition.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to get the item from.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <param name="predicate">The specified condition.</param>
        /// <returns>A random item from the <see cref="IEnumerable{T}"/> matching the given condition.</returns>
        public static T Random<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) =>
            (enumerable as T[] ?? enumerable.ToArray()) is { Length: > 0 } arr ? arr.Where(predicate).Random() : default;

        /// <summary>
        /// Retrieves a random item from an <see cref="IEnumerable{T}"/>.
        /// <para>
        /// <br>Unlike <see cref="Random{T}(IEnumerable{T})"/>, this method optimizes performance</br>
        /// <br>by pre-allocating memory for an array, resulting in faster computation and iteration.</br>
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to select the item from.</param>
        /// <returns>A randomly selected item from the <see cref="IEnumerable{T}"/>.</returns>
        public static T RandomAlloc<T>(this IEnumerable<T> enumerable)
        {
            T[] array = enumerable as T[] ?? enumerable.ToArray();
            return !array.Any() ? default : array.ElementAt(UnityEngine.Random.Range(0, array.Length));
        }

        /// <summary>
        /// Retrieves a random item from an <see cref="IEnumerable{T}"/> based on a specified condition.
        /// <para>
        /// <br>Unlike <see cref="Random{T}(IEnumerable{T}, Func{T, bool})"/>, this method optimizes performance</br>
        /// <br>by pre-allocating memory for an array, resulting in faster computation and iteration.</br>
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to select the item from.</param>
        /// <param name="predicate">The specified condition for item selection.</param>
        /// <returns>A randomly selected item from the <see cref="IEnumerable{T}"/> that meets the given condition.</returns>
        public static T RandomAlloc<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            T[] arr = enumerable.Where(predicate) as T[] ?? enumerable.Where(predicate).ToArray();
            return !arr.Any() ? default : arr.ElementAt(UnityEngine.Random.Range(0, arr.Length));
        }

        /// <summary>
        /// Shuffles an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="iterations">The amount of times to repeat the shuffle operation.</param>
        /// <returns>A shuffled version of the <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, int iterations = 1)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));
            if (iterations < 1)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            T[] array = enumerable.ToArray();
            array.Shuffle(iterations);
            return array;
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to iterate over.</param>
        /// <param name="action">The <see cref="Action{T}"/> delegate to apply to each element.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/>.</returns>
        /// <remarks>
        /// This extension method is designed for performing side effects on each element
        /// of the collection without creating a new collection. It does not modify the
        /// collection itself and is typically used for its side effects, such as logging,
        /// printing, or updating state.
        /// </remarks>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
                action(item);

            return enumerable;
        }

        /// <summary>
        /// Adds a collection of items to an existing <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="enumerable">The original <see cref="IEnumerable{T}"/> to which items will be added.</param>
        /// <param name="collection">The collection of items to add.</param>
        /// <returns>The modified <see cref="IEnumerable{T}"/> after adding the items.</returns>
        public static IEnumerable<T> AddRange<T>(this IEnumerable<T> enumerable, IEnumerable<T> collection)
        {
            IEnumerable<T> result = enumerable;

            foreach (T item in collection)
                result.AddItem(item);

            return result;
        }

        /// <summary>
        /// Adds a collection of items to an existing array of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="array">The original array of <typeparamref name="T"/> to which items will be added.</param>
        /// <param name="collection">The collection of items to add.</param>
        /// <returns>The modified array of <typeparamref name="T"/> after adding the items.</returns>
        public static T[] AddRange<T>(this T[] array, IEnumerable<T> collection)
        {
            foreach (T item in collection)
                array.AddItem(item);

            return array;
        }

        /// <summary>
        /// Adds a collection of items to an existing <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="hashset">The original <see cref="HashSet{T}"/> to which items will be added.</param>
        /// <param name="collection">The collection of items to add.</param>
        /// <returns>The modified <see cref="HashSet{T}"/> after adding the items.</returns>
        public static HashSet<T> AddRange<T>(this HashSet<T> hashset, IEnumerable<T> collection)
        {
            foreach (T item in collection)
                hashset.Add(item);

            return hashset;
        }

        /// <summary>
        /// Tries to add the specified key-value pair to the dictionary. Returns <see langword="false"/> if the key already exists.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to which the key-value pair is to be added.</param>
        /// <param name="kvp">The key-value pair to add to the dictionary.</param>
        /// <returns><see langword="true"/> if the key-value pair was successfully added; otherwise, <see langword="false"/> if the key already exists in the dictionary.</returns>
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> kvp)
        {
            if (dictionary.ContainsKey(kvp.Key))
                return false;

            dictionary.Add(kvp.Key, kvp.Value);
            return true;
        }

        /// <summary>
        /// Tries to add the specified key and value to the dictionary. Returns <see langword="false"/> if the key already exists.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to which the key-value pair is to be added.</param>
        /// <param name="key">The key to add to the dictionary.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns><see langword="true"/> if the key-value pair was successfully added; otherwise, <see langword="false"/> if the key already exists in the dictionary.</returns>
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) => TryAdd(dictionary, new KeyValuePair<TKey, TValue>(key, value));
    }
}