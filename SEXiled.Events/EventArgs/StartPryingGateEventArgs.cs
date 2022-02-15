// -----------------------------------------------------------------------
// <copyright file="StartPryingGateEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    using Interactables.Interobjects;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Contains all information before SCP-096 begins prying a gate open.
    /// </summary>
    public class StartPryingGateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartPryingGateEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="scp096"><inheritdoc cref="Scp096"/></param>
        /// <param name="gate"><inheritdoc cref="Gate"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StartPryingGateEventArgs(Scp096 scp096, Player player, PryableDoor gate, bool isAllowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Gate = Door.Get(gate);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the SCP-096 instance.
        /// </summary>
        public Scp096 Scp096 { get; }

        /// <summary>
        /// Gets the player that is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="PryableDoor"/> to be pried open.
        /// </summary>
        public Door Gate { get; }

        /// <summary>
        /// Gets or Sets a value indicating whether or not the gate can be pried open by SCP-096.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
