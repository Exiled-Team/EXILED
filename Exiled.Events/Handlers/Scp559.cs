// -----------------------------------------------------------------------
// <copyright file="Scp559.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp559;
    using Exiled.Events.Features;
#pragma warning disable SA1623

    /// <summary>
    /// All SCP-559 related events.
    /// </summary>
    public static class Scp559
    {
        /// <summary>
        /// Invoked before SCP-559 spawns.
        /// </summary>
        public static Event<SpawningEventArgs> Spawning { get; set; } = new();

        /// <summary>
        /// Invoked before player interacts with SCP-559.
        /// </summary>
        public static Event<InteractingScp559EventArgs> Interacting { get; set; } = new();

        /// <summary>
        /// Called before SCP-559 spawns.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningEventArgs"/> instance.</param>
        public static void OnSpawning(SpawningEventArgs ev) => Spawning.InvokeSafely(ev);

        /// <summary>
        /// Called before player interacts with SCP-559.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingScp559EventArgs"/> instance.</param>
        public static void OnInteracting(InteractingScp559EventArgs ev) => Interacting.InvokeSafely(ev);
    }
}