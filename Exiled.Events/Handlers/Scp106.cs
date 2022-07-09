// -----------------------------------------------------------------------
// <copyright file="Scp106.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-106 related events.
    /// </summary>
    public static class Scp106
    {
        /// <summary>
        /// Gets or sets the event invoked before SCP-106 creates a portal.
        /// </summary>
        public static Event<CreatingPortalEventArgs> CreatingPortal { get; set; } = new();

        /// <summary>
        /// Gets or sets the event invoked before SCP-106 teleports using a portal.
        /// </summary>
        public static Event<TeleportingEventArgs> Teleporting { get; set; } = new();

        /// <summary>
        /// Gets or sets the event invoked before containing SCP-106.
        /// </summary>
        public static Event<ContainingEventArgs> Containing { get; set; } = new();

        /// <summary>
        /// Called before SCP-106 creates a portal.
        /// </summary>
        /// <param name="ev">The <see cref="CreatingPortalEventArgs"/> instance.</param>
        public static void OnCreatingPortal(CreatingPortalEventArgs ev) => CreatingPortal.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-106 teleports using a portal.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs"/> instance.</param>
        public static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);

        /// <summary>
        /// Called before containing SCP-106.
        /// </summary>
        /// <param name="ev">The <see cref="ContainingEventArgs"/> instance.</param>
        public static void OnContaining(ContainingEventArgs ev) => Containing.InvokeSafely(ev);
    }
}
