// -----------------------------------------------------------------------
// <copyright file="Scp3114.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp244;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Features;

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
        ///     Called before diguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="DisguisingEventArgs" /> instance.</param>
        public static void OnDisguising(DisguisingEventArgs ev) => Disguising.InvokeSafely(ev);

        /// <summary>
        ///     Called before diguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="DisguisedEventArgs" /> instance.</param>
        public static void OnDisguised(DisguisedEventArgs ev) => Disguised.InvokeSafely(ev);

        /// <summary>
        ///     Called before diguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="TryUseBodyEventArgs" /> instance.</param>
        public static void OnTryUseBody(TryUseBodyEventArgs ev) => TryUseBody.InvokeSafely(ev);

        /// <summary>
        ///     Called before diguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="RevealedEventArgs" /> instance.</param>
        public static void OnRevealed(RevealedEventArgs ev) => Revealed.InvokeSafely(ev);

        /// <summary>
        ///     Called before diguising to a new Roles.
        /// </summary>
        /// <param name="ev">The <see cref="RevealingEventArgs" /> instance.</param>
        public static void OnRevealing(RevealingEventArgs ev) => Revealing.InvokeSafely(ev);
    }
}