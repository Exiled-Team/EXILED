// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Warhead;
    using Exiled.Events.Extensions;

    using static Events;

    /// <summary>
    ///     Handles warhead related events.
    /// </summary>
    public static class Warhead
    {
        /// <summary>
        ///     Invoked before stopping the warhead.
        /// </summary>
        public static event CustomEventHandler<StoppingWarheadEventArgs> StoppingWarhead;

        /// <summary>
        ///     Invoked before starting the warhead.
        /// </summary>
        public static event CustomEventHandler<StartingWarheadEventArgs> StartingWarhead;

        /// <summary>
        ///     Invoked after the warhead has been detonated.
        /// </summary>
        public static event CustomEventHandler DetonatedWarhead;

        /// <summary>
        ///     Invoked before changing the warhead lever status.
        /// </summary>
        public static event CustomEventHandler<ChangingLeverStatusEventArgs> ChangingLeverStatus;

        /// <summary>
        ///     Called before stopping the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingWarheadEventArgs" /> instance.</param>
        public static void OnStoppingWarhead(StoppingWarheadEventArgs ev)
        {
            StoppingWarhead.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before starting the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StartingWarheadEventArgs" /> instance.</param>
        public static void OnStartingWarhead(StartingWarheadEventArgs ev)
        {
            StartingWarhead.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after the warhead has been detonated.
        /// </summary>
        public static void OnDetonated()
        {
            DetonatedWarhead.InvokeSafely();
        }

        /// <summary>
        ///     Called before changing the warhead lever status.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingLeverStatusEventArgs" /> instance.</param>
        public static void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev)
        {
            ChangingLeverStatus.InvokeSafely(ev);
        }
    }
}