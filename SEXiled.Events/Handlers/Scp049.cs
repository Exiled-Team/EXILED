// -----------------------------------------------------------------------
// <copyright file="Scp049.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Handlers
{
    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Extensions;

    using static SEXiled.Events.Events;

    /// <summary>
    /// SCP-049 related events.
    /// </summary>
    public static class Scp049
    {
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
        /// Called before SCP-049 starts to recall a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartingRecallEventArgs"/> instance.</param>
        public static void OnStartingRecall(StartingRecallEventArgs ev) => StartingRecall.InvokeSafely(ev);
    }
}
