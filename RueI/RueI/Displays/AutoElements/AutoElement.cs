namespace RueI.Displays;

using PlayerRoles;

using RueI.Displays.Scheduling;
using RueI.Elements;
using RueI.Extensions;

/// <summary>
/// Manages and automatically assigns <see cref="Element"/> instances to any <see cref="DisplayCore"/> meeting a criteria.
/// </summary>
/// <remarks>
/// An <see cref="AutoElement"/> puts an <see cref="Element"/> in a <see cref="DisplayCore"/> if they match a
/// <see cref="Displays.Roles"/>. You can use the <see cref="AutoElement(Roles, Element)"/> constructor for an
/// <see cref="AutoElement"/> that assigns a single instance of an <see cref="Element"/>. On the other hand,
/// if you want to create an element for each player, you can use the <see cref="AutoElement(Roles, Func{DisplayCore, Element})"/>
/// constructor.
/// </remarks>
public class AutoElement
{
    /// <summary>
    /// Represents a periodic update for an <see cref="AutoElement"/>.
    /// </summary>
    /// <param name="time">How often the <see cref="AutoElement"/> should schedule an auto-update.</param>
    /// <param name="priority">The priority of the scheduled job.</param>
    public record PeriodicUpdate(TimeSpan time, int priority = 10);

    private const int AUTOUPDATEPRIORITY = 5;

    private static readonly List<AutoElement> AutoElements = new();

    private readonly Element? element;
    private readonly Func<DisplayCore, Element>? creator;

    private readonly IElemReference<Element> reference = DisplayCore.GetReference<Element>();

    private (PeriodicUpdate update, JobToken token)? autoUpdate = null;

    static AutoElement()
    {
        PlayerRoleManager.OnRoleChanged += OnRoleChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoElement"/> class.
    /// </summary>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="creator">A <see cref="Func{T, TResult}"/> that creates the elements.</param>
    public AutoElement(Roles roles, Func<DisplayCore, Element> creator)
    {
        this.creator = creator;
        Roles = roles;

        AutoElements.Add(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoElement"/> class.
    /// </summary>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="element">The element to automatically give.</param>
    public AutoElement(Roles roles, Element element)
    {
        this.element = element;
        Roles = roles;

        AutoElements.Add(this);
    }

    /// <summary>
    /// Gets or sets the roles that this <see cref="AutoElement"/> will give this element on.
    /// </summary>
    public Roles Roles { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="PeriodicUpdate"/> indicating how often this <see cref="AutoElement"/> should
    /// schedule an update for players with the element, or <c>null</c> if it should not auto-update.
    /// </summary>
    public PeriodicUpdate? UpdateEvery
    {
        get => autoUpdate?.update;

        set
        {
            if (value != null)
            {
                autoUpdate = (value, new());
            }
            else
            {
                autoUpdate = null;
            }
        }
    }

    /// <summary>
    /// Disables this <see cref="AutoElement"/>.
    /// </summary>
    public virtual void Disable()
    {
        AutoElements.Remove(this);
    }

    /// <summary>
    /// Gives this <see cref="AutoElement"/> to a <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to give to.</param>
    protected virtual void GiveTo(DisplayCore core)
    {
        if (element != null)
        {
            core.AddAsReference(reference, element);
        }
        else
        {
            core.AddAsReference(reference, creator!(core));
        }

        if (autoUpdate != null)
        {
            ScheduleUpdate(core, autoUpdate.Value);
        }
    }

    /// <summary>
    /// Removes this <see cref="AutoElement"/> from a <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to give to.</param>
    protected virtual void RemoveFrom(DisplayCore core)
    {
        core.RemoveReference(reference);

        if (autoUpdate != null)
        {
            core.Scheduler.KillJob(autoUpdate.Value.token);
        }
    }

    private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
    {
        if (prevRole.RoleTypeId != newRole.RoleTypeId)
        {
            DisplayCore core = DisplayCore.Get(hub);

            foreach (AutoElement autoElement in AutoElements)
            {
                if (autoElement.Roles.HasFlagFast(newRole.RoleTypeId))
                {
                    autoElement.GiveTo(core);
                }
                else
                {
                    autoElement.RemoveFrom(core);
                }
            }

            core.Update(AUTOUPDATEPRIORITY);
        }
    }

    private static void ScheduleUpdate(DisplayCore core, (PeriodicUpdate update, JobToken token) autoUpdate)
    {
        core.Scheduler.Schedule(autoUpdate.update.time, () => ScheduleUpdate(core, autoUpdate), autoUpdate.token);
    }
}