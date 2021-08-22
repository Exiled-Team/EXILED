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

    using Interactables.Interobjects;

    /// <summary>
    /// Contains all information before SCP-096 begins prying a gate open.
    /// </summary>
    public class StartPryingGateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartPryingGateEventArgs"/> class.
        /// </summary>
        /// <param name="scp096">The Scp096 who is triggering the event.</param>
        /// <param name="gate">The gate to be pried open.</param>
        public StartPryingGateEventArgs(Player scp096, PryableDoor gate)
        {
            Player = scp096;
            Gate = gate;
        }

        /// <summary>
        /// Gets the player that is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="PryableDoor"/> to be pried open.
        /// </summary>
        public PryableDoor Gate { get; }

        /// <summary>
        /// Gets or Sets a value indicating whether or not the gate can be pried open by SCP-096.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
