// -----------------------------------------------------------------------
// <copyright file="Scp106.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Features;

    /// <summary>
    ///     SCP-106 related events.
    /// </summary>
    public static class Scp106
    {
        /// <summary>
        ///     Gets or sets invoked before SCP-106 teleports using the hunter atlas.
        /// </summary>
        public static Event<TeleportingEventArgs> Teleporting { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before SCP-106 use the stalk ability.
        /// </summary>
        public static Event<StalkingEventArgs> Stalking { get; set; } = new();

        /// <summary>
        ///     Called before SCP-106 teleports using the hunter atlas.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs" /> instance.</param>
        public static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-106 use the stalk ability.
        /// </summary>
        /// <param name="ev">The <see cref="StalkingEventArgs"/> instance.</param>
        public static void OnStalking(StalkingEventArgs ev) => Stalking.InvokeSafely(ev);
    }
}