// -----------------------------------------------------------------------
// <copyright file="Scp106.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp106;

    using Extensions;

    using static Events;

    /// <summary>
    ///     SCP-106 related events.
    /// </summary>
    public static class Scp106
    {
        /// <summary>
        ///     Invoked before SCP-106 teleports through the minimap.
        /// </summary>
        public static event CustomEventHandler<TeleportingEventArgs> Teleporting;

        /// <summary>
        ///     Invoked before SCP-106 stalks
        /// </summary>
        public static event CustomEventHandler<PlayerTryEnterStalkEventArgs> Stalking;

        /// <summary>
        ///     Invoked before SCP-106 leaves stalks
        /// </summary>
        public static event CustomEventHandler<PlayerTryLeaveStalkEventArgs> LeaveStalk;

        /// <summary>
        ///     Invoked before Server code for SCP-106 changes stalk status
        /// </summary>
        public static event CustomEventHandler<ServerChangingStalk> ServerChangingStalk;

        /// <summary>
        ///     Called before SCP-106 teleports through the minimap.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs" /> instance.</param>
        public static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-106 teleports through the minimap.
        /// </summary>
        /// <param name="ev">The <see cref="PlayerTryEnterStalkEventArgs" /> instance.</param>
        public static void OnStalking(PlayerTryEnterStalkEventArgs ev) => Stalking.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-106 teleports through the minimap.
        /// </summary>
        /// <param name="ev">The <see cref="PlayerTryEnterStalkEventArgs" /> instance.</param>
        public static void OnLeavingStalk(PlayerTryLeaveStalkEventArgs ev) => LeaveStalk.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-106 teleports through the minimap.
        /// </summary>
        /// <param name="ev">The <see cref="ServerChangingStalk" /> instance.</param>
        public static void OnServerChangingStalk(ServerChangingStalk ev) => ServerChangingStalk.InvokeSafely(ev);
    }
}