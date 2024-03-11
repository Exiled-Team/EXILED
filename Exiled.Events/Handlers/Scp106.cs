// -----------------------------------------------------------------------
// <copyright file="Scp106.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-106 related events.
    /// </summary>
    public static class Scp106
    {
        /// <summary>
        /// Invoked before SCP-106 attacks player.
        /// </summary>
        public static Event<AttackingEventArgs> Attacking { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-106 teleports using the hunter atlas.
        /// </summary>
        public static Event<TeleportingEventArgs> Teleporting { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-106 use the stalk ability.
        /// </summary>
        public static Event<StalkingEventArgs> Stalking { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-106 exit the stalk ability.
        /// </summary>
        public static Event<ExitStalkingEventArgs> ExitStalking { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-106 passes a door.
        /// </summary>
        public static Event<PassingDoorEventArgs> PassingDoor { get; set; } = new();

        /// <summary>
        /// Called before SCP-106 attacks player.
        /// </summary>
        /// <param name="ev">The <see cref="AttackingEventArgs" /> instance.</param>
        public static void OnAttacking(AttackingEventArgs ev) => Attacking.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-106 teleports using the hunter atlas.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs" /> instance.</param>
        public static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-106 use the stalk ability.
        /// </summary>
        /// <param name="ev">The <see cref="StalkingEventArgs"/> instance.</param>
        public static void OnStalking(StalkingEventArgs ev) => Stalking.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-106 exit the stalk ability.
        /// </summary>
        /// <param name="ev">The <see cref="ExitStalkingEventArgs"/> instance.</param>
        public static void OnExitStalking(ExitStalkingEventArgs ev) => ExitStalking.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-106 passes a door.
        /// </summary>
        /// <param name="ev">The <see cref="PassingDoorEventArgs"/> instance.</param>
        public static void OnPassingDoor(PassingDoorEventArgs ev) => PassingDoor.InvokeSafely(ev);
    }
}