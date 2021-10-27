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
        /// Test.
        /// </summary>
        public static event CustomEventHandler<EatingSCP330EventArgs> Eating;

        /// <summary>
        /// Called after player eats SCP330.
        /// </summary>
        /// <param name="ev">The <see cref="EatingSCP330EventArgs"/> instance.</param>
        public static void OnEating(EatingSCP330EventArgs ev) => Eating.InvokeSafely(ev);
    }
}
