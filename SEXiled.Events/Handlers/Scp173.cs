// -----------------------------------------------------------------------
// <copyright file="Scp173.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Handlers
{
    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Extensions;

    using static SEXiled.Events.Events;

    /// <summary>
    /// SCP-173 related events.
    /// </summary>
    public static class Scp173
    {
        /// <summary>
        /// Invoked before players near SCP-173 blink.
        /// </summary>
        public static event CustomEventHandler<BlinkingEventArgs> Blinking;

        /// <summary>
        /// Invoked before a tantrum is placed.
        /// </summary>
        public static event CustomEventHandler<PlacingTantrumEventArgs> PlacingTantrum;

        /// <summary>
        /// Called before players near SCP-173 blink.
        /// </summary>
        /// <param name="ev">The <see cref="BlinkingEventArgs"/> instance.</param>
        public static void OnBlinking(BlinkingEventArgs ev) => Blinking.InvokeSafely(ev);

        /// <summary>
        /// Called before a tantrum is placed.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingTantrumEventArgs"/> instance.</param>
        public static void OnPlacingTantrum(PlacingTantrumEventArgs ev) => PlacingTantrum.InvokeSafely(ev);
    }
}
