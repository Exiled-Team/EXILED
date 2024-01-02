// -----------------------------------------------------------------------
// <copyright file="Scp2536.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp2536;
    using Exiled.Events.Features;
#pragma warning disable SA1623

    /// <summary>
    /// SCP-2536 related events.
    /// </summary>
    public static class Scp2536
    {
        /// <summary>
        /// Invoked before SCP-2536 chooses target to spawn.
        /// </summary>
        public static Event<FindingPositionEventArgs> FindingPosition { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-2536 gives a gift to a player.
        /// </summary>
        public static Event<GrantingGiftEventArgs> GrantingGift { get; set; } = new();

        /// <summary>
        /// Called before SCP-2536 chooses a target.
        /// </summary>
        /// <param name="ev">The <see cref="FindingPositionEventArgs"/> instance.</param>
        public static void OnFindingPosition(FindingPositionEventArgs ev) => FindingPosition.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-2536 gives a gift to a player.
        /// </summary>
        /// <param name="ev">The <see cref="GrantingGiftEventArgs"/> instance.</param>
        public static void OnGrantingGift(GrantingGiftEventArgs ev) => GrantingGift.InvokeSafely(ev);
    }
}