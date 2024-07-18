namespace RueI.Displays;

using JetBrains.Annotations;
using RueI.Displays.Scheduling;
using RueI.Elements;
using RueI.Extensions;

/// <summary>
/// Manages all of the <see cref="DisplayBase"/>s for a <see cref="ReferenceHub"/>.
/// </summary>
public class DisplayCore
{
    // TODO: make this not break if someone rejoins
    private readonly List<DisplayBase> displays = new();
    private readonly Dictionary<IElemReference<Element>, Element> referencedElements = new();

    static DisplayCore()
    {
        RoundRestarting.RoundRestart.OnRestartTriggered += OnRestart;
        ReferenceHub.OnPlayerRemoved += OnPlayerRemoved;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayCore"/> class.
    /// </summary>
    /// <param name="hub">The hub to create the display for.</param>
    protected DisplayCore(ReferenceHub hub)
    {
        Hub = hub;

        if (hub != null)
        {
            DisplayCores.Add(hub, this);
        }

        Scheduler = new(this);

        UnityAlternative.Provider.LogDebug("Generated new display core for " + (hub?.name ?? "null"));
    }

    /// <summary>
    /// Gets the <see cref="Scheduling.Scheduler"/> for this <see cref="DisplayCore"/>.
    /// </summary>
    public Scheduler Scheduler { get; }

    /// <summary>
    /// Gets the <see cref="ReferenceHub"/> that this display is for.
    /// </summary>
    public ReferenceHub Hub { get; }

    /// <summary>
    /// Gets a dictionary containing the DisplayCores for each ReferenceHub.
    /// </summary>
    internal static Dictionary<ReferenceHub, DisplayCore> DisplayCores { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether or not updates will currently be ignored.
    /// </summary>
    internal bool IgnoreUpdate { get; set; } = false;

    /// <summary>
    /// Gets a DisplayCore from a <see cref="ReferenceHub"/>, or creates it if it doesn't exist.
    /// </summary>
    /// <param name="hub">The hub to get the display for.</param>
    /// <returns>The DisplayCore.</returns>
    /// <remarks>This method will never fail nor return null.</remarks>
    public static DisplayCore Get(ReferenceHub hub)
    {
        if (DisplayCores.TryGetValue(hub, out DisplayCore value))
        {
            return value;
        }
        else
        {
            return new DisplayCore(hub);
        }
    }

    /// <summary>
    /// Gets a new <see cref="IElemReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the element.</typeparam>
    /// <returns>A new, unique <see cref="IElemReference{T}"/>.</returns>
    public static IElemReference<T> GetReference<T>()
        where T : Element
    {
        return new ElemReference<T>();
    }

    /// <summary>
    /// Updates this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="priority">The priority of the update - defaults to 10.</param>
    public void Update(int priority = 10)
    {
        if (IgnoreUpdate)
        {
            return;
        }

        Scheduler.ScheduleUpdate(TimeSpan.FromMilliseconds(50), priority);
    }

    /// <summary>
    /// Gets an <see cref="Element"/> as <typeparamref name="T"/> if the <see cref="IElemReference{T}"/> exists within this <see cref="DisplayCore"/>'s element references.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to get.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <returns>The instance of <typeparamref name="T"/> if the <see cref="Element"/> exists within the <see cref="DisplayCore"/>'s element references, otherwise null.</returns>
    public T? GetElement<T>(IElemReference<T> reference)
        where T : Element
    {
        if (referencedElements.TryGetValue(reference, out Element value))
        {
            return value as T;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets an <see cref="Element"/> as <typeparamref name="T"/>, or creates it.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to get.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <param name="creator">A function that creates a new instance of <typeparamref name="T"/> if it does not exist.</param>
    /// <returns>The instance of <typeparamref name="T"/>.</returns>
    public T GetElementOrNew<T>(IElemReference<T> reference, Func<T> creator)
        where T : Element
    {
        if (referencedElements.TryGetValue(reference, out Element value) && value is T casted)
        {
            return casted;
        }
        else
        {
            T created = creator();
            referencedElements.Add(reference, created);
            return created;
        }
    }

    /// <summary>
    /// Adds an <see cref="Element"/> as an <see cref="IElemReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to add.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <param name="element">The <see cref="Element"/> to add.</param>
    public void AddAsReference<T>(IElemReference<T> reference, T element)
        where T : Element
    {
        referencedElements[reference] = element;
    }

    /// <summary>
    /// Removes a <see cref="IElemReference{T}"/> from this <see cref="DisplayCore"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to remove.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to remove.</param>
    public void RemoveReference<T>(IElemReference<T> reference)
        where T : Element
    {
        _ = referencedElements.Remove(reference);
    }

    /// <summary>
    /// Gets the text of this <see cref="DisplayCore"/>.
    /// </summary>
    /// <returns>The text that is combined to become a single hint.</returns>
    public string GetText()
    {
        return ElemCombiner.Combine(this, GetAllElements());
    }

    /// <summary>
    /// Updates this display, skipping all checks.
    /// </summary>
    internal void InternalUpdate()
    {
        string text = ElemCombiner.Combine(this, GetAllElements());

        UnityAlternative.Provider.ShowHint(Hub, text);
        UnityAlternative.Provider.LogDebug($"Updating display for {Hub?.name}");

        Events.Events.OnDisplayUpdated(new(this));
    }

    /// <summary>
    /// Adds a display to this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="display">The display to add.</param>
    internal void AddDisplay(DisplayBase display) => displays.Add(display);

    /// <summary>
    /// Removes a display from this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="display">The display to remove.</param>
    internal void RemoveDisplay(DisplayBase display) => displays.Remove(display);

    /// <summary>
    /// Cleans up the dictionary after the server restarts.
    /// </summary>
    private static void OnRestart()
    {
        DisplayCores.Clear();
    }

    private static void OnPlayerRemoved(ReferenceHub hub)
    {
        DisplayCores.Remove(hub);
    }

    private IEnumerable<Element> GetAllElements() => displays.SelectMany(x => x.GetAllElements())
                                                      .Concat(referencedElements.Values.FilterDisabled());

    private class ElemReference<T> : IElemReference<T>
        where T : Element
    {
    }
}