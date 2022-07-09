// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// Handles warhead related events.
    /// </summary>
    public static class Warhead
    {
        /// <summary>
        /// Invoked before stopping the warhead.
        /// </summary>
        public static readonly Event<StoppingEventArgs> Stopping = new();

        /// <summary>
        /// Invoked before starting the warhead.
        /// </summary>
        public static readonly Event<StartingEventArgs> Starting = new();

        /// <summary>
        /// Invoked after the warhead has been detonated.
        /// </summary>
        public static readonly Event Detonated = new();

        /// <summary>
        /// Invoked before changing the warhead lever status.
        /// </summary>
        public static readonly Event<ChangingLeverStatusEventArgs> ChangingLeverStatus = new();

        /// <summary>
        /// Called before stopping the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingEventArgs"/> instance.</param>
        public static void OnStopping(StoppingEventArgs ev) => Stopping.InvokeSafely(ev);

        /// <summary>
        /// Called before starting the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StartingEventArgs"/> instance.</param>
        public static void OnStarting(StartingEventArgs ev) => Starting.InvokeSafely(ev);

        /// <summary>
        /// Called after the warhead has been detonated.
        /// </summary>
        public static void OnDetonated() => Detonated.InvokeSafely();

        /// <summary>
        /// Called before changing the warhead lever status.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingLeverStatusEventArgs"/> instance.</param>
        public static void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev) => ChangingLeverStatus.InvokeSafely(ev);
    }
}
