namespace RueI.Extensions;

/// <summary>
/// Provides extensions for working with all types.
/// </summary>
internal static class UniversalExtensions
{
    /// <summary>
    /// Adds this instance to an <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of this instance and the collection to add to.</typeparam>
    /// <param name="item">The instance to add.</param>
    /// <param name="collection">The collection to add the elements to.</param>
    /// <returns>A reference to <paramref name="item"/>.</returns>
    public static T AddTo<T>(this T item, ICollection<T> collection)
        where T : class
    {
        collection.Add(item);
        return item;
    }
}
