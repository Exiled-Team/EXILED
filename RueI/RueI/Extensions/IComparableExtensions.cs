namespace RueI.Extensions;

/// <summary>
/// Provides extensions and helpers for working with classes that implement <see cref="IComparable{T}"/>.
/// </summary>
internal static class IComparableExtensions
{
    /// <summary>
    /// Gets the maximum of two <see cref="IComparable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to use.</typeparam>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <returns>The maximum of the two.</returns>
    public static T Max<T>(this T first, T second)
        where T : IComparable<T>
    {
        if (first.CompareTo(second) > 0)
        {
            return first;
        }
        else
        {
            return second;
        }
    }

    /// <summary>
    /// Gets the maximum of two <see cref="IComparable{T}"/>, if a bool is true.
    /// </summary>
    /// <typeparam name="T">The type to use.</typeparam>
    /// <param name="first">The first value.</param>
    /// <param name="check">Whether or not to perform the check.</param>
    /// <param name="second">The second value.</param>
    /// <returns>The maximum of the two, or the first value if the bool is true.</returns>
    public static T MaxIf<T>(this T first, bool check, T second)
        where T : IComparable<T>
    {
        return check ? first.Max(second) : first;
    }
}