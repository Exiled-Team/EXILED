// -----------------------------------------------------------------------
// <copyright file="Scp049.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp049;

    using Extensions;

    using static Events;

    /// <summary>
    ///     SCP-049 related events.
    /// </summary>
    public static class Scp049
    {
        /// <summary>
        ///     Invoked before SCP-049 finishes recalling a player.
        /// </summary>
        public static event CustomEventHandler<FinishingRecallEventArgs> FinishingRecall;

        /// <summary>
        ///     Invoked before SCP-049 begins recalling a player.
        /// </summary>
        public static event CustomEventHandler<StartingRecallEventArgs> StartingRecall;

        /// <summary>
        ///     Invoked before SCP-049 uses the sense ability.
        /// </summary>
        public static event CustomEventHandler<ActivatingSenseEventArgs> ActivatingSense;

        /// <summary>
        ///     Invoked before SCP-049 uses the call ability.
        /// </summary>
        public static event CustomEventHandler<SendingCallEventArgs> SendingCall;

        /// <summary>
        /// Invoked before SCP-049 attacks player.
        /// </summary>
        public static event CustomEventHandler<AttackingEventArgs> Attacking;

        /// <summary>
        ///     Called before SCP-049 finishes recalling a player.
        /// </summary>
        /// <param name="ev">The <see cref="FinishingRecallEventArgs" /> instance.</param>
        public static void OnFinishingRecall(FinishingRecallEventArgs ev) => FinishingRecall.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-049 starts to recall a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartingRecallEventArgs" /> instance.</param>
        public static void OnStartingRecall(StartingRecallEventArgs ev) => StartingRecall.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-049 starts the sense ability.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingSenseEventArgs" /> instance.</param>
        public static void OnActivatingSense(ActivatingSenseEventArgs ev) => ActivatingSense.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-049 starts the call ability.
        /// </summary>
        /// <param name="ev">The <see cref="SendingCallEventArgs" /> instance.</param>
        public static void OnSendingCall(SendingCallEventArgs ev) => SendingCall.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-049 attacks player.
        /// </summary>
        /// <param name="ev">The <see cref="AttackingEventArgs"/> instance.</param>
        public static void OnAttacking(AttackingEventArgs ev) => Attacking.InvokeSafely(ev);
    }
}