// -----------------------------------------------------------------------
// <copyright file="Scp1507.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp1507;
    using Exiled.Events.Features;

#pragma warning disable SA1623

    /// <summary>
    /// SCP-1507 related events.
    /// </summary>
    public static class Scp1507
    {
        /// <summary>
        /// Invokes before SCP-1507 attacks door.
        /// </summary>
        public static Event<AttackingDoorEventArgs> AttackingDoor { get; set; } = new();

        /// <summary>
        /// Called before SCP-1507 attacks door.
        /// </summary>
        /// <param name="ev">The <see cref="AttackingDoorEventArgs"/> instance.</param>
        public static void OnAttackingDoor(AttackingDoorEventArgs ev) => AttackingDoor.InvokeSafely(ev);
    }
}