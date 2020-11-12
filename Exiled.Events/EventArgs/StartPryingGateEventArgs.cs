// -----------------------------------------------------------------------
// <copyright file="StartPryingGateEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Contains all information before SCP-096 starts prying a gate open.
    /// </summary>
    public class StartPryingGateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartPryingGateEventArgs"/> class.
        /// </summary>
        /// <param name="scp096">The Scp096 who is triggering the event.</param>
        /// <param name="player">The player that is playing SCP-096.</param>
        /// <param name="gate">The gate to be pried open.</param>
        public StartPryingGateEventArgs(Scp096 scp096, Player player, Door gate)
        {
            Scp096 = scp096;
            Player = player;
            Gate = gate;
        }

        /// <summary>
        /// Gets the Scp096 object.
        /// </summary>
        public Scp096 Scp096 { get; }

        /// <summary>
        /// Gets the player who is playing SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="Door"/> to be pried open.
        /// </summary>
        public Door Gate { get; }

        /// <summary>
        /// Gets or Sets a value indicating whether or not they should be allowed to pry the gate open.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
