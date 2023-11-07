namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Door;
#pragma warning disable SA1623 // Property summary documentation should match accessors
    using Exiled.Events.EventArgs.Item;

    using Exiled.Events.Features;
    using Exiled.Events.Patches.Events.Item;

    /// <summary>
    ///     Door related events.
    /// </summary>

    public class Door
    {
        /// <summary>
        /// Invoked before a <see cref="API.Features.Items.Jailbird"/> is charged.
        /// </summary>
        public static Event<DoorDamagingEventArgs> DoorDamaging { get; set; } = new();
        /// <summary>
        /// Invoked before a <see cref="API.Features.Items.Jailbird"/> is charged.
        /// </summary>
        public static Event<DoorDestroyingEventArgs> DoorDestroying { get; set; } = new();
        /// <summary>
        /// Invoked before a <see cref="API.Features.Items.Jailbird"/> is charged.
        /// </summary>
        public static Event<DoorDestroyedEventArgs> DoorDestroyed { get; set; } = new();
        /// <summary>
        /// Called before destroyed the door
        /// </summary>
        /// <param name="ev">The <see cref="DoorDestroyedEventArgs"/> instance.</param>
        public static void OnDoorDestroyed(DoorDestroyedEventArgs ev) => DoorDestroyed.InvokeSafely(ev);
        /// <summary>
        /// Called before destroying the door
        /// </summary>
        /// <param name="ev">The <see cref="DoorDestroyingEventArgs"/> instance.</param>
        public static void OnDoorDestroying(DoorDestroyingEventArgs ev) => DoorDestroying.InvokeSafely(ev);
        /// <summary>
        /// Called before dealing damage to the door
        /// </summary>
        /// <param name="ev">The <see cref="DoorDamagingEventArgs"/> instance.</param>
        public static void OnDoorDamaging(DoorDamagingEventArgs ev) => DoorDamaging.InvokeSafely(ev);
    }
}
