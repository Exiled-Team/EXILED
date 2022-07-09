// -----------------------------------------------------------------------
// <copyright file="Scp173.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-173 related events.
    /// </summary>
    public static class Scp173
    {
        /// <summary>
        /// Invoked before players near SCP-173 blink.
        /// </summary>
        public static readonly Event<BlinkingEventArgs> Blinking = new();

        /// <summary>
        /// Invoked before a tantrum is placed.
        /// </summary>
        public static readonly Event<PlacingTantrumEventArgs> PlacingTantrum = new();

        /// <summary>
        /// Invoked before using breakneck speeds.
        /// </summary>
        public static readonly Event<UsingBreakneckSpeedsEventArgs> UsingBreakneckSpeeds = new();

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

        /// <summary>
        /// Called before a using breakneck speeds.
        /// </summary>
        /// <param name="ev">The <see cref="UsingBreakneckSpeedsEventArgs"/> instance.</param>
        public static void OnUsingBreakneckSpeeds(UsingBreakneckSpeedsEventArgs ev) => UsingBreakneckSpeeds.InvokeSafely(ev);
    }
}
