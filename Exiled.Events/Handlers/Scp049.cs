// -----------------------------------------------------------------------
// <copyright file="Scp049.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using static Exiled.Events.Events;

    /// <summary>
    /// SCP-049 related events.
    /// </summary>
    public class Scp049
    {
        /// <summary>
        /// Invoked before a player is infected.
        /// </summary>
        public static event CustomEventHandler<InfectPlayerArgs> InfectPlayer;

        /// <summary>
        /// Invoked before SCP049 starts to infect a player.
        /// </summary>
        public static event CustomEventHandler<StartInfectPlayerArgs> StartInfectPlayer;

        /// <summary>
        /// Invoked before a player is infected.
        /// </summary>
        /// <param name="ev">The <see cref="InfectPlayerArgs"/> instance.</param>
        public static void OnInfectPlayer(InfectPlayerArgs ev) => InfectPlayer.InvokeSafely(ev);

        /// <summary>
        /// Invoked before SCP049 starts to infect a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartInfectPlayerArgs"/> instance.</param>
        public static void OnStartInfectPlayer(StartInfectPlayerArgs ev) => StartInfectPlayer.InvokeSafely(ev);
    }
}
