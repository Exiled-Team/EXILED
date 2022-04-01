// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers {
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Handles warhead related events.
    /// </summary>
    public static class Warhead {
        /// <summary>
        /// Invoked before stopping the warhead.
        /// </summary>
        public static event CustomEventHandler<StoppingEventArgs> Stopping;

        /// <summary>
        /// Invoked before starting the warhead.
        /// </summary>
        public static event CustomEventHandler<StartingEventArgs> Starting;

        /// <summary>
        /// Invoked after the warhead has been detonated.
        /// </summary>
        public static event CustomEventHandler Detonated;

        /// <summary>
        /// Invoked before changing the warhead lever status.
        /// </summary>
        public static event CustomEventHandler<ChangingLeverStatusEventArgs> ChangingLeverStatus;

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
