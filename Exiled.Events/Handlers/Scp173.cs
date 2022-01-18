// -----------------------------------------------------------------------
// <copyright file="Scp173.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Events;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;
    using static Exiled.Events.Events;

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
        public static void OnBlinking(BlinkingEventArgs ev) => EventManager.Instance.Invoke<BlinkingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnBlinking(BlinkingEventArgs ev) => Blinking.InvokeSafely(ev);


        /// <summary>
        /// Called before a tantrum is placed.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingTantrumEventArgs"/> instance.</param>
        public static void OnPlacingTantrum(PlacingTantrumEventArgs ev) => EventManager.Instance.Invoke<PlacingTantrumEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnPlacingTantrum(PlacingTantrumEventArgs ev) => PlacingTantrum.InvokeSafely(ev);

    }
}
