// -----------------------------------------------------------------------
// <copyright file="InteractingTeslaEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    using TeslaGate = TeslaGate;

    /// <summary>
    /// Contains all information before SCP-079 triggers a tesla gate.
    /// </summary>
    public class InteractingTeslaEventArgs : IScp079Event, ITeslaEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingTeslaEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="teslaGate">
        /// <inheritdoc cref="Tesla" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        /// <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        public InteractingTeslaEventArgs(Player player, TeslaGate teslaGate, float auxiliaryPowerCost)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            Tesla = API.Features.TeslaGate.Get(teslaGate);
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = auxiliaryPowerCost <= Scp079.Energy;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.TeslaGate" /> that SCP-079 is triggering.
        /// </summary>
        public API.Features.TeslaGate Tesla { get; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to interact with a tesla gate through SCP-079.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-079 can interact with the tesla gate.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}