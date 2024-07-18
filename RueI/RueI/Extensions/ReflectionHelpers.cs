namespace RueI.Extensions;

using RueI.Displays;
using RueI.Displays.Scheduling;
using RueI.Elements;

/// <summary>
/// Contains methods designed for use by reflection.
/// </summary>
public static class ReflectionHelpers
{
    /// <summary>
    /// Gets an <see cref="Action{T1, T2, T3, T4}"/> that can be used to add an element, with a <see cref="TimedElemRef{T}"/> as a closure.
    /// </summary>
    /// <returns>An <see cref="Action{T1, T2, T3, T4}"/> that can be used to add an element to a <see cref="ReferenceHub"/>.</returns>
    /// <remarks>
    /// Every time this method is called, it creates a new <see cref="TimedElemRef{T}"/>. Therefore, every delegate returned by this method
    /// represents a unique 'element'.
    /// </remarks>
    public static Action<ReferenceHub, string, float, TimeSpan> GetElementShower()
    {
        TimedElemRef<SetElement> elemRef = new();
        return (hub, content, name, span) => DisplayCore.Get(hub).SetElemTempFunctional(content, name, span, elemRef);
    }

    /// <summary>
    /// Gets a <see cref="Func{T}"/> that can be used to easily call <see cref="GetElementShower"/>.
    /// </summary>
    /// <returns>A <see cref="Func{T}"/> that itself returns an <see cref="Action{T1, T2, T3, T4}"/>.</returns>
    /// <remarks>
    /// The <see cref="Func{T}"/> returned by this method is identical to calling <see cref="GetElementShower"/>.
    /// This method serves a helper to easily turn GetElementShower into a <see cref="Func{T}"/>, to make
    /// reflection easier.
    /// </remarks>
    public static Func<Action<ReferenceHub, string, float, TimeSpan>> GetElemCreator() => GetElementShower;
}
