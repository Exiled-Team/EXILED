// -----------------------------------------------------------------------
// <copyright file="Scp049.cs" company="Exiled Team">
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
        public static void OnFinishingRecall(FinishingRecallEventArgs ev) => EventManager.Instance.Invoke<FinishingRecallEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnFinishingRecall(FinishingRecallEventArgs ev) => FinishingRecall.InvokeSafely(ev);


        /// <summary>
        /// Called before SCP-049 starts to recall a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartingRecallEventArgs"/> instance.</param>
        public static void OnStartingRecall(StartingRecallEventArgs ev) => EventManager.Instance.Invoke<StartingRecallEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnStartingRecall(StartingRecallEventArgs ev) => StartingRecall.InvokeSafely(ev);

    }
}
