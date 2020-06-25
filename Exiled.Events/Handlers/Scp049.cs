// -----------------------------------------------------------------------
// <copyright file="Scp049.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using static Exiled.Events.Events;

    /// <summary>
    /// Scp049 related events.
    /// </summary>
    public class Scp049
    {
        /// <summary>
        /// Invoked before a player is infected.
        /// </summary>
        public static event CustomEventHandler<FinishingRecallEventArgs> FinishingRecall;

        /// <summary>
        /// Invoked before Scp049 starts to infect a player.
        /// </summary>
        public static event CustomEventHandler<StartingRecallEventArgs> StartingRecall;

        /// <summary>
        /// Invoked before a player is recalled.
        /// </summary>
        /// <param name="ev">The <see cref="FinishingRecallEventArgs"/> instance.</param>
        public static void OnFinishingRecall(FinishingRecallEventArgs ev) => FinishingRecall.InvokeSafely(ev);

        /// <summary>
        /// Invoked before Scp049 starts to recall a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartingRecallEventArgs"/> instance.</param>
        public static void OnStartingRecall(StartingRecallEventArgs ev) => StartingRecall.InvokeSafely(ev);
    }
}
