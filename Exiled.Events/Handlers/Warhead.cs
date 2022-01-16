// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Utils;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Handles warhead related events.
    /// </summary>
    public static class Warhead
    {
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
        public static void OnStopping(StoppingEventArgs ev) => EventManager.Instance.Invoke<StoppingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnStopping(StoppingEventArgs ev) => Stopping.InvokeSafely(ev);


        /// <summary>
        /// Called before starting the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StartingEventArgs"/> instance.</param>
        public static void OnStarting(StartingEventArgs ev) => EventManager.Instance.Invoke<StartingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnStarting(StartingEventArgs ev) => Starting.InvokeSafely(ev);


        /// <summary>
        /// Called after the warhead has been detonated.
        /// </summary>
        public static void OnDetonated() => Detonated.InvokeSafely();

        /// <summary>
        /// Called before changing the warhead lever status.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingLeverStatusEventArgs"/> instance.</param>
        public static void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev) => EventManager.Instance.Invoke<ChangingLeverStatusEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingLeverStatus(ChangingLeverStatusEventArgs ev) => ChangingLeverStatus.InvokeSafely(ev);

    }
}
