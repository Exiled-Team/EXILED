// -----------------------------------------------------------------------
// <copyright file="Scp3114.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp3114;
    using Features;

    /// <summary>
    ///     Scp3114 related events.
    /// </summary>
    public static class Scp3114
    {
        /// <summary>
        ///     Invoked before Disguising.
        /// </summary>
        public static Event<DisguisingEventArgs> Disguising { get; set; } = new();

        /// <summary>
        ///     Invoked before Disguising.
        /// </summary>
        public static Event<DisguisedEventArgs> Disguised { get; set; } = new();

        /// <summary>
        ///     Invoked before Disguising.
        /// </summary>
        public static Event<TryUseBodyEventArgs> TryUseBody { get; set; } = new();

        /// <summary>
        ///     Invoked before Disguising.
        /// </summary>
        public static Event<RevealedEventArgs> Revealed { get; set; } = new();

        /// <summary>
        ///     Invoked before Disguising.
        /// </summary>
        public static Event<RevealingEventArgs> Revealing { get; set; } = new();

        /// <summary>
        ///     Invoked before Strangling.
        /// </summary>
        public static Event<StranglingEventArgs> Strangling { get; set; } = new();

        /// <summary>
        ///     Invoked before a player is slapped.
        /// </summary>
        public static Event<SlappingPlayerEventArgs> SlappingPlayer { get; set; } = new();

        /// <summary>
        ///     Invoked before triggering the strangle cooldown.
        /// </summary>
        public static Event<StranglingFinishedEventArgs> StranglingFinished { get; set; } = new();

        /// <summary>
        ///     Called before disguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="DisguisingEventArgs" /> instance.</param>
        public static void OnDisguising(DisguisingEventArgs ev) => Disguising.InvokeSafely(ev);

        /// <summary>
        ///     Called before disguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="DisguisedEventArgs" /> instance.</param>
        public static void OnDisguised(DisguisedEventArgs ev) => Disguised.InvokeSafely(ev);

        /// <summary>
        ///     Called before disguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="TryUseBodyEventArgs" /> instance.</param>
        public static void OnTryUseBody(TryUseBodyEventArgs ev) => TryUseBody.InvokeSafely(ev);

        /// <summary>
        ///     Called before disguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="RevealedEventArgs" /> instance.</param>
        public static void OnRevealed(RevealedEventArgs ev) => Revealed.InvokeSafely(ev);

        /// <summary>
        ///     Called before disguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="RevealingEventArgs" /> instance.</param>
        public static void OnRevealing(RevealingEventArgs ev) => Revealing.InvokeSafely(ev);

        /// <summary>
        ///     Called before strangling a player.
        /// </summary>
        /// <param name="ev">The <see cref="StranglingEventArgs"/> instance.</param>
        public static void OnStrangling(StranglingEventArgs ev) => Strangling.InvokeSafely(ev);

        /// <summary>
        ///     Called after Scp3114 finishes strangling a player.
        /// </summary>
        /// <param name="ev">The <see cref="StranglingFinishedEventArgs" /> instance.</param>
        public static void OnStranglingFinished(StranglingFinishedEventArgs ev) => StranglingFinished.InvokeSafely(ev);

        /// <summary>
        ///     Called before Scp3114 slaps a player.
        /// </summary>
        /// <param name="ev">The <see cref="SlappingPlayerEventArgs" /> instance.</param>
        public static void OnSlappingPlayer(SlappingPlayerEventArgs ev) => SlappingPlayer.InvokeSafely(ev);
    }
}
