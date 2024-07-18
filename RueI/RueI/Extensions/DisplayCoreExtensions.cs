namespace RueI.Extensions;

using RueI.Displays;
using RueI.Displays.Scheduling;
using RueI.Elements;
using RueI.Elements.Interfaces;

/// <summary>
/// Provides miscellaneous utility for <see cref="DisplayCore"/>s.
/// </summary>
public static class DisplayCoreExtensions
{
    /// <summary>
    /// Sets the content of a <see cref="SetElement"/> <see cref="IElemReference{T}"/>, or creates it.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to use.</param>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <param name="content">The new content of the <see cref="SetElement"/>.</param>
    /// <param name="position">The position of the <see cref="SetElement"/> if it needs to be created.</param>
    public static void SetElementOrNew(this DisplayCore core, IElemReference<SetElement> reference, string content, float position)
    {
        SetElement? element = core.GetElement(reference);
        if (element != null)
        {
            element.Content = content;
            element.Position = position;
        }
        else
        {
            SetElement newElem = new(position, content);
            core.AddAsReference(reference, newElem);
        }
    }

    /// <summary>
    /// Temporarily adds an <see cref="Element"/> to a <see cref="DisplayCore"/> using the specified <see cref="TimedElemRef{T}"/>.
    /// This method updates the <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to add the element to.</param>
    /// <param name="element">The element to add.</param>
    /// <param name="time">How long to add the element to the <see cref="DisplayCore"/> for.</param>
    /// <param name="elemRef">The <see cref="TimedElemRef{T}"/> to use.</param>
    /// <typeparam name="T">The type of the element.</typeparam>
    public static void AddTemp<T>(this DisplayCore core, T element, TimeSpan time, TimedElemRef<T> elemRef)
        where T : Element, ISettable
    {
        core.AddAsReference(elemRef, element);

        core.Scheduler.ReplaceJob(time, () => core.RemoveReference(elemRef), elemRef.JobToken);

        core.Update();
    }

    /// <summary>
    /// Temporarily adds a <see cref="SetElement"/> using the provided string and position, or sets it if it already exists.
    /// This method updates the <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to add the element to.</param>
    /// <param name="content">The content of the element.</param>
    /// <param name="position">The position of the element.</param>
    /// <param name="time">How long to keep the element in the <see cref="DisplayCore"/> for.</param>
    /// <param name="elemRef">The <see cref="TimedElemRef{T}"/> to use.</param>
    public static void SetElemTemp(this DisplayCore core, string content, float position, TimeSpan time, TimedElemRef<SetElement> elemRef)
    {
        core.SetElementOrNew(elemRef, content, position);

        core.Scheduler.ReplaceJob(time, () => core.RemoveReference(elemRef), elemRef.JobToken);

        core.Update();
    }

    /// <summary>
    /// Temporarily adds a <see cref="SetElement"/> using the provided string and functional position, or sets it if it already exists.
    /// This method updates the <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to add the element to.</param>
    /// <param name="content">The content of the element.</param>
    /// <param name="position">The position of the element.</param>
    /// <param name="time">How long to keep the element in the <see cref="DisplayCore"/> for.</param>
    /// <param name="elemRef">The <see cref="TimedElemRef{T}"/> to use.</param>
    public static void SetElemTempFunctional(this DisplayCore core, string content, float position, TimeSpan time, TimedElemRef<SetElement> elemRef)
    {
        core.SetElementOrNew(elemRef, content, Ruetility.FunctionalToScaledPosition(position));

        core.Scheduler.ReplaceJob(time, () => core.RemoveReference(elemRef), elemRef.JobToken);

        core.Update();
    }
}
