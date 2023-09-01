// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Warhead;
    using Exiled.Events.Features;

    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;

    /// <summary>
    ///     Handles warhead related events.
    /// </summary>
    public class Warhead
    {
        /// <summary>
        ///     Invoked before stopping the warhead.
        /// </summary>
        public static Event<StoppingEventArgs> Stopping { get; set; } = new();

        /// <summary>
        ///     Invoked before starting the warhead.
        /// </summary>
        public static Event<StartingEventArgs> Starting { get; set; } = new();

        /// <summary>
        ///     Invoked before changing the warhead lever status.
        /// </summary>
        public static Event<ChangingLeverStatusEventArgs> ChangingLeverStatus { get; set; } = new();

        /// <summary>
        ///     Invoked after the warhead has been detonated.
        /// </summary>
        public static Event Detonated { get; set; } = new();

        /// <summary>
        ///     Invoked before detonating the warhead.
        /// </summary>
        public static Event<DetonatingEventArgs> Detonating { get; set; } = new();

        /// <summary>
        ///     Called before stopping the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingEventArgs" /> instance.</param>
        public static void OnStopping(StoppingEventArgs ev) => Stopping.InvokeSafely(ev);

        /// <summary>
        ///     Called before starting the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StartingEventArgs" /> instance.</param>
        public static void OnStarting(StartingEventArgs ev) => Starting.InvokeSafely(ev);

        /// <summary>
        ///     Called before changing the warhead lever status.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingLeverStatusEventArgs" /> instance.</param>
        public static void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev) => ChangingLeverStatus.InvokeSafely(ev);

        /// <summary>
        ///     Called after the warhead has been detonated.
        /// </summary>
        public static void OnDetonated() => Detonated.InvokeSafely();

        /// <summary>
        ///     Called before detonating the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="DetonatingEventArgs"/> instance.</param>
        public static void OnDetonating(DetonatingEventArgs ev) => Detonating.InvokeSafely(ev);
    }
}