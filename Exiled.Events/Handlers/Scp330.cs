// -----------------------------------------------------------------------
// <copyright file="Scp330.cs" company="Exiled Team">
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
    /// Scp330 related events.
    /// </summary>
    public static class Scp330
    {
        /// <summary>
        /// Invoked before a player eats a candy from SCP-330.
        /// </summary>
        public static event CustomEventHandler<EatingScp330EventArgs> EatingScp330;

        /// <summary>
        /// Invoked after the player has eaten a candy from SCP-330.
        /// </summary>
        public static event CustomEventHandler<EatenScp330EventArgs> EatenScp330;

        /// <summary>
        /// Called before a player eats a candy from SCP-330.
        /// </summary>
        /// <param name="ev">The <see cref="EatingScp330EventArgs"/> instance.</param>
        public static void OnEatingScp330(EatingScp330EventArgs ev) => EventManager.Instance.Invoke<EatingScp330EventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnEatingScp330(EatingScp330EventArgs ev) => EatingScp330.InvokeSafely(ev);


        /// <summary>
        /// Called after the player has eaten a candy from SCP-330.
        /// </summary>
        /// <param name="ev">The <see cref="EatenScp330EventArgs"/> instance.</param>
        public static void OnEatenScp330(EatenScp330EventArgs ev) => EventManager.Instance.Invoke<EatenScp330EventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnEatenScp330(EatenScp330EventArgs ev) => EatenScp330.InvokeSafely(ev);

    }
}
