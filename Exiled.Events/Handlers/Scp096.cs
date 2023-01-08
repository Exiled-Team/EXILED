// -----------------------------------------------------------------------
// <copyright file="Scp096.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp096;

    using Extensions;

    using static Events;

    /// <summary>
    ///     SCP-096 related events.
    /// </summary>
    public static class Scp096
    {
        /// <summary>
        ///     Invoked before SCP-096 is enraged.
        /// </summary>
        public static event CustomEventHandler<EnragingEventArgs> Enraging;

        /// <summary>
        ///     Invoked before SCP-096 calms down.
        /// </summary>
        public static event CustomEventHandler<CalmingDownEventArgs> CalmingDown;

        /// <summary>
        ///     Invoked before adding a target to SCP-096.
        /// </summary>
        public static event CustomEventHandler<AddingTargetEventArgs> AddingTarget;

        /// <summary>
        ///     Invoked before SCP-096 begins prying open a gate.
        /// </summary>
        public static event CustomEventHandler<StartPryingGateEventArgs> StartPryingGate;

        /// <summary>
        ///     Invoked before SCP-096 begins charging.
        /// </summary>
        public static event CustomEventHandler<ChargingEventArgs> Charging;

        /// <summary>
        ///     Invoked before SCP-096 tries not to cry.
        /// </summary>
        public static event CustomEventHandler<TryingNotToCryEventArgs> TryingNotToCry;

        /// <summary>
        ///     Called before SCP-096 is enraged.
        /// </summary>
        /// <param name="ev">The <see cref="EnragingEventArgs" /> instance.</param>
        public static void OnEnraging(EnragingEventArgs ev) => Enraging.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-096 calms down.
        /// </summary>
        /// <param name="ev">The <see cref="CalmingDownEventArgs" /> instance.</param>
        public static void OnCalmingDown(CalmingDownEventArgs ev) => CalmingDown.InvokeSafely(ev);

        /// <summary>
        ///     Called before adding a target to SCP-096.
        /// </summary>
        /// <param name="ev">The <see cref="AddingTargetEventArgs" /> instance.</param>
        public static void OnAddingTarget(AddingTargetEventArgs ev) => AddingTarget.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-096 begins prying open a gate.
        /// </summary>
        /// <param name="ev">The <see cref="StartPryingGateEventArgs" /> instance.</param>
        public static void OnStartPryingGate(StartPryingGateEventArgs ev) => StartPryingGate.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-096 begins charging.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingEventArgs" /> instance.</param>
        public static void OnCharging(ChargingEventArgs ev) => Charging.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-096 starts trying not to cry.
        /// </summary>
        /// <param name="ev">The <see cref="TryingNotToCryEventArgs" /> instance.</param>
        public static void OnTryingNotToCry(TryingNotToCryEventArgs ev) => TryingNotToCry.InvokeSafely(ev);
    }
}