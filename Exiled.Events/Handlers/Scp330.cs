// -----------------------------------------------------------------------
// <copyright file="Scp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// SCP330 related events.
    /// </summary>
    public static class Scp330
    {
        /// <summary>
        /// Invoked before player ate SCP330.
        /// </summary>
        public static event CustomEventHandler<EatingScp330EventArgs> Eating;

        /// <summary>
        /// Invoked after player ate SCP330.
        /// </summary>
        public static event CustomEventHandler<EatenScp330EventArgs> Eaten;

        /// <summary>
        /// Called before player ate SCP330.
        /// </summary>
        /// <param name="ev">The <see cref="EatingScp330EventArgs"/> instance.</param>
        public static void OnEating(EatingScp330EventArgs ev) => Eating.InvokeSafely(ev);

        /// <summary>
        /// Called after player ate SCP330.
        /// </summary>
        /// <param name="ev">The <see cref="EatenScp330EventArgs"/> instance.</param>
        public static void OnEaten(EatenScp330EventArgs ev) => Eaten.InvokeSafely(ev);
    }
}
