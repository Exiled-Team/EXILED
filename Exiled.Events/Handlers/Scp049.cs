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
        public static event CustomEventHandler<StartingReviveEventArgs> StartingRecall;

        /// <summary>
        ///     Invoked before SCP-049 sends call to player.
        /// </summary>
        public static event CustomEventHandler<Sending049CallEventArgs> SendingRecall;

        /// <summary>
        ///     Invoked before SCP-049 uses sense.
        /// </summary>
        public static event CustomEventHandler<DoctorSenseEventArgs> DoctorSense;

        /// <summary>
        ///     Invoked before SCP-0492 can consume
        /// </summary>
        public static event CustomEventHandler<ZombieConsumeEventArgs> StartingZombieConsume;

        /// <summary>
        ///     Called before SCP-049 finishes recalling a player.
        /// </summary>
        /// <param name="ev">The <see cref="FinishingRecallEventArgs" /> instance.</param>
        public static void OnFinishingRecall(FinishingRecallEventArgs ev) => FinishingRecall.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-049 starts to recall a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartingReviveEventArgs" /> instance.</param>
        public static void OnStartingRecall(StartingReviveEventArgs ev) => StartingRecall.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-049 can start recall via send to client.
        /// </summary>
        /// <param name="ev">The <see cref="Sending049CallEventArgs" /> instance.</param>
        public static void OnSendingCall(Sending049CallEventArgs ev) => SendingRecall.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-049 starts doctor sense.
        /// </summary>
        /// <param name="ev">The <see cref="DoctorSenseEventArgs" /> instance.</param>
        public static void OnDoctorSense(DoctorSenseEventArgs ev) => DoctorSense.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-0492 can start consuming.
        /// </summary>
        /// <param name="ev">The <see cref="ZombieConsumeEventArgs" /> instance.</param>
        public static void OnStartingConsume(ZombieConsumeEventArgs ev) => StartingZombieConsume.InvokeSafely(ev);
    }
}