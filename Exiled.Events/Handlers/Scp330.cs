// -----------------------------------------------------------------------
// <copyright file="Scp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp330;

    using Extensions;

    using static Events;

    /// <summary>
    ///     Scp330 related events.
    /// </summary>
    public static class Scp330
    {
        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> interacts with SCP-330.
        /// </summary>
        public static event CustomEventHandler<InteractingScp330EventArgs> InteractingScp330;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> drop a SCP-330 candy.
        /// </summary>
        public static event CustomEventHandler<DroppingScp330EventArgs> DroppingScp330;

        /// <summary>
        ///     Invoked before a player eats a candy from SCP-330.
        /// </summary>
        public static event CustomEventHandler<EatingScp330EventArgs> EatingScp330;

        /// <summary>
        ///     Invoked after the player has eaten a candy from SCP-330.
        /// </summary>
        public static event CustomEventHandler<EatenScp330EventArgs> EatenScp330;

        /// <summary>
        ///     Called before a player eats a candy from SCP-330.
        /// </summary>
        /// <param name="ev">The <see cref="EatingScp330EventArgs" /> instance.</param>
        public static void OnEatingScp330(EatingScp330EventArgs ev) => EatingScp330.InvokeSafely(ev);

        /// <summary>
        ///     Called after the player has eaten a candy from SCP-330.
        /// </summary>
        /// <param name="ev">The <see cref="EatenScp330EventArgs" /> instance.</param>
        public static void OnEatenScp330(EatenScp330EventArgs ev) => EatenScp330.InvokeSafely(ev);

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> interacts with SCP-330.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingScp330EventArgs" /> instance.</param>
        public static void OnInteractingScp330(InteractingScp330EventArgs ev) => InteractingScp330.InvokeSafely(ev);

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> searches a Pickup.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingScp330EventArgs" /> instance.</param>
        public static void OnDroppingScp330(DroppingScp330EventArgs ev) => DroppingScp330.InvokeSafely(ev);
    }
}