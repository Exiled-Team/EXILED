namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Features;
    using Exiled.Events.Patches.Events.Item;

    /// <summary>
    ///     Door related events.
    /// </summary>

    public class Door
    {
        /// <summary>
        /// Called before dealing damage to the door
        /// </summary>
        public static Event<DamagingDoorEventArgs> DoorDamaging { get; set; } = new();
        /// <summary>
        /// Called before destroying the door
        /// </summary>
        public static Event<DestroyingDoorEventArgs> DoorDestroying { get; set; } = new();
        /// <summary>
        /// Called before destroyed the door
        /// </summary>
        public static Event<DestroyedDoorEventArgs> DoorDestroyed { get; set; } = new();
        /// <summary>
        /// Called before destroyed the door
        /// </summary>
        /// <param name="ev">The <see cref="DestroyedDoorEventArgs"/> instance.</param>
        public static void OnDoorDestroyed(DestroyedDoorEventArgs ev) => DoorDestroyed.InvokeSafely(ev);
        /// <summary>
        /// Called before destroying the door
        /// </summary>
        /// <param name="ev">The <see cref="DestroyingDoorEventArgs"/> instance.</param>
        public static void OnDoorDestroying(DestroyingDoorEventArgs ev) => DoorDestroying.InvokeSafely(ev);
        /// <summary>
        /// Called before dealing damage to the door
        /// </summary>
        /// <param name="ev">The <see cref="DamagingDoorEventArgs"/> instance.</param>
        public static void OnDoorDamaging(DamagingDoorEventArgs ev) => DoorDamaging.InvokeSafely(ev);
    }
}
