// -----------------------------------------------------------------------
// <copyright file="Scp096.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-096 related events.
    /// </summary>
    public static class Scp096
    {
        /// <summary>
        /// Invoked before SCP-096 is enraged.
        /// </summary>
        public static readonly Event<EnragingEventArgs> Enraging = new();

        /// <summary>
        /// Invoked before SCP-096 calms down.
        /// </summary>
        public static readonly Event<CalmingDownEventArgs> CalmingDown = new();

        /// <summary>
        /// Invoked before adding a target to SCP-096.
        /// </summary>
        public static readonly Event<AddingTargetEventArgs> AddingTarget = new();

        /// <summary>
        /// Invoked before SCP-096 begins prying open a gate.
        /// </summary>
        public static readonly Event<StartPryingGateEventArgs> StartPryingGate = new();

        /// <summary>
        /// Invoked before SCP-096 begins charging.
        /// </summary>
        public static readonly Event<ChargingEventArgs> Charging = new();

        /// <summary>
        /// Invoked before SCP-096 tries not to cry.
        /// </summary>
        public static readonly Event<TryingNotToCryEventArgs> TryingNotToCry = new();

        /// <summary>
        /// Invoked before SCP-096 charges a player.
        /// </summary>
        public static readonly Event<ChargingPlayerEventArgs> ChargingPlayer = new();

        /// <summary>
        /// Called before SCP-096 is enraged.
        /// </summary>
        /// <param name="ev">The <see cref="EnragingEventArgs"/> instance.</param>
        public static void OnEnraging(EnragingEventArgs ev) => Enraging.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 calms down.
        /// </summary>
        /// <param name="ev">The <see cref="CalmingDownEventArgs"/> instance.</param>
        public static void OnCalmingDown(CalmingDownEventArgs ev) => CalmingDown.InvokeSafely(ev);

        /// <summary>
        /// Called before adding a target to SCP-096.
        /// </summary>
        /// <param name="ev">The <see cref="AddingTargetEventArgs"/> instance.</param>
        public static void OnAddingTarget(AddingTargetEventArgs ev) => AddingTarget.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 begins prying open a gate.
        /// </summary>
        /// <param name="ev">The <see cref="StartPryingGateEventArgs"/> instance.</param>
        public static void OnStartPryingGate(StartPryingGateEventArgs ev) => StartPryingGate.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 begins charging.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingEventArgs"/> instance.</param>
        public static void OnCharging(ChargingEventArgs ev) => Charging.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 starts trying not to cry.
        /// </summary>
        /// <param name="ev">The <see cref="TryingNotToCryEventArgs"/> instance.</param>
        public static void OnTryingNotToCry(TryingNotToCryEventArgs ev) => TryingNotToCry.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-096 charges a player.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingPlayerEventArgs"/> instance.</param>
        public static void OnChargingPlayer(ChargingPlayerEventArgs ev) => ChargingPlayer.InvokeSafely(ev);
    }
}
