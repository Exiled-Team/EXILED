// -----------------------------------------------------------------------
// <copyright file="Scp096.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp096;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-096 related events.
    /// </summary>
    public static class Scp096
    {
        /// <summary>
        /// Invoked before SCP-096 is enraged.
        /// </summary>
        public static Event<EnragingEventArgs> Enraging { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-096 calms down.
        /// </summary>
        public static Event<CalmingDownEventArgs> CalmingDown { get; set; } = new();

        /// <summary>
        /// Invoked before adding a target to SCP-096.
        /// </summary>
        public static Event<AddingTargetEventArgs> AddingTarget { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-096 begins prying open a gate.
        /// </summary>
        public static Event<StartPryingGateEventArgs> StartPryingGate { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-096 begins charging.
        /// </summary>
        public static Event<ChargingEventArgs> Charging { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-096 tries not to cry.
        /// </summary>
        public static Event<TryingNotToCryEventArgs> TryingNotToCry { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-096 damages someone.
        /// </summary>
        public static Event<AttackingEventArgs> Attacking { get; set; } = new();

        /// <summary>
        /// Called before SCP-096 is enraged.
        /// </summary>
        /// <param name="ev">The <see cref="EnragingEventArgs" /> instance.</param>
        public static void OnEnraging(EnragingEventArgs ev) => Enraging.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 calms down.
        /// </summary>
        /// <param name="ev">The <see cref="CalmingDownEventArgs" /> instance.</param>
        public static void OnCalmingDown(CalmingDownEventArgs ev) => CalmingDown.InvokeSafely(ev);

        /// <summary>
        /// Called before adding a target to SCP-096.
        /// </summary>
        /// <param name="ev">The <see cref="AddingTargetEventArgs" /> instance.</param>
        public static void OnAddingTarget(AddingTargetEventArgs ev) => AddingTarget.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 begins prying open a gate.
        /// </summary>
        /// <param name="ev">The <see cref="StartPryingGateEventArgs" /> instance.</param>
        public static void OnStartPryingGate(StartPryingGateEventArgs ev) => StartPryingGate.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 begins charging.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingEventArgs" /> instance.</param>
        public static void OnCharging(ChargingEventArgs ev) => Charging.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 starts trying not to cry.
        /// </summary>
        /// <param name="ev">The <see cref="TryingNotToCryEventArgs" /> instance.</param>
        public static void OnTryingNotToCry(TryingNotToCryEventArgs ev) => TryingNotToCry.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 damages someone.
        /// </summary>
        /// <param name="ev">The <see cref="AttackingEventArgs"/> instance.</param>
        public static void OnAttacking(AttackingEventArgs ev) => Attacking.InvokeSafely(ev);
    }
}