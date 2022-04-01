// -----------------------------------------------------------------------
// <copyright file="InteractingTeslaEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-079 triggers a tesla gate.
    /// </summary>
    public class InteractingTeslaEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingTeslaEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="tesla"><inheritdoc cref="Tesla"/></param>
        /// <param name="auxiliaryPowerCost"><inheritdoc cref="AuxiliaryPowerCost"/></param>
        public InteractingTeslaEventArgs(Player player, TeslaGate tesla, float auxiliaryPowerCost) {
            Player = player;
            Tesla = tesla;
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = auxiliaryPowerCost <= player.Energy;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="TeslaGate"/> that SCP-079 is triggering.
        /// </summary>
        public TeslaGate Tesla { get; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to interact with a tesla gate through SCP-079.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 can interact with the tesla gate.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
