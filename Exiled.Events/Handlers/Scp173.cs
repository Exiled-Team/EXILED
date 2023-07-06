// -----------------------------------------------------------------------
// <copyright file="Scp173.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp173;
    using Exiled.Events.Features;

    /// <summary>
    ///     SCP-173 related events.
    /// </summary>
    public static class Scp173
    {
        /// <summary>
        ///     Gets or sets invoked before players near SCP-173 blink.
        /// </summary>
        public static Event<BlinkingEventArgs> Blinking { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before a tantrum is placed.
        /// </summary>
        public static Event<PlacingTantrumEventArgs> PlacingTantrum { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before using breakneck speeds.
        /// </summary>
        public static Event<UsingBreakneckSpeedsEventArgs> UsingBreakneckSpeeds { get; set; } = new();

        /// <summary>
        ///     Called before players near SCP-173 blink.
        /// </summary>
        /// <param name="ev">The <see cref="BlinkingEventArgs" /> instance.</param>
        public static void OnBlinking(BlinkingEventArgs ev) => Blinking.InvokeSafely(ev);

        /// <summary>
        ///     Called before a tantrum is placed.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingTantrumEventArgs" /> instance.</param>
        public static void OnPlacingTantrum(PlacingTantrumEventArgs ev) => PlacingTantrum.InvokeSafely(ev);

        /// <summary>
        ///     Called before a using breakneck speeds.
        /// </summary>
        /// <param name="ev">The <see cref="UsingBreakneckSpeedsEventArgs" /> instance.</param>
        public static void OnUsingBreakneckSpeeds(UsingBreakneckSpeedsEventArgs ev) => UsingBreakneckSpeeds.InvokeSafely(ev);
    }
}