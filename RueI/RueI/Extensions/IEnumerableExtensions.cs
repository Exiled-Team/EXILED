namespace RueI.Extensions;

/// <summary>
/// Provides extensions for working with collections.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Determines if a <see cref="IEnumerable{T}"/> has only one element that passes a filter.
    /// </summary>
    /// <typeparam name="T">The inner type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to check.</param>
    /// <param name="filter">A filter to use.</param>
    /// <returns>true if there is only one element in the <see cref="IEnumerable{T}"/> and that element passes the filter, otherwise false.</returns>
    public static bool Only<T>(this IEnumerable<T> enumerable, Func<T, bool> filter)
    {
        using IEnumerator<T> enumerator = enumerable.GetEnumerator();
        return enumerator.MoveNext() && !filter(enumerator.Current) && !enumerator.MoveNext();
    }

    /// <summary>
    /// Determines if a <see cref="IEnumerable{T}"/> has only one element.
    /// </summary>
    /// <typeparam name="T">The inner type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to check.</param>
    /// <returns>true if there is only one element in the <see cref="IEnumerable{T}"/>.</returns>
    public static bool Only<T>(this IEnumerable<T> enumerable)
    {
        using IEnumerator<T> enumerator = enumerable.GetEnumerator();
        return enumerator.MoveNext() && !enumerator.MoveNext();
    }

    /// <summary>
    /// Gets a <see cref="IEnumerable{T}"/> of values from a <see cref="IDictionary{TKey, TValue}"/> using the key.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary's key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary's value.</typeparam>
    /// <param name="dict">The dictionary to use.</param>
    /// <param name="key">The key to search with.</param>
    /// <returns>The <see cref="IEnumerable{T}"/>, or <see cref="Enumerable.Empty{TResult}"/> if it is not found.</returns>
    public static IEnumerable<TValue> GetMultiple<TKey, TValue>(this IDictionary<TKey, IEnumerable<TValue>> dict, TKey key)
    {
        return dict.TryGetValue(key, out IEnumerable<TValue> value) ? value : Enumerable.Empty<TValue>();
    }
}
