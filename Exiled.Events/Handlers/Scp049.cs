// -----------------------------------------------------------------------
// <copyright file="Scp049.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers {
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// SCP-049 related events.
    /// </summary>
    public static class Scp049 {
        /// <summary>
        /// Invoked before SCP-049 finishes recalling a player.
        /// </summary>
        public static event CustomEventHandler<FinishingRecallEventArgs> FinishingRecall;

        /// <summary>
        /// Invoked before SCP-049 begins recalling a player.
        /// </summary>
        public static event CustomEventHandler<StartingRecallEventArgs> StartingRecall;

        /// <summary>
        /// Called before SCP-049 finishes recalling a player.
        /// </summary>
        /// <param name="ev">The <see cref="FinishingRecallEventArgs"/> instance.</param>
        public static void OnFinishingRecall(FinishingRecallEventArgs ev) => FinishingRecall.InvokeSafely(ev);

        /// <summary>
        /// Called before Scp049 starts to recall a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartingRecallEventArgs"/> instance.</param>
        public static void OnStartingRecall(StartingRecallEventArgs ev) => StartingRecall.InvokeSafely(ev);
    }
}
