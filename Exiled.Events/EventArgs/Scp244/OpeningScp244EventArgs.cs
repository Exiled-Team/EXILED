// -----------------------------------------------------------------------
// <copyright file="OpeningScp244EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp244
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.Usables.Scp244;

    /// <summary>
    ///     Contains all information before a player opens SCP-244.
    /// </summary>
    public class OpeningScp244EventArgs : IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OpeningScp244EventArgs" /> class.
        /// </summary>
        /// <param name="pickup">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        public OpeningScp244EventArgs(Scp244DeployablePickup pickup)
        {
            Pickup = (Scp244Pickup)API.Features.Pickups.Pickup.Get(pickup);
        }

        /// <summary>
        /// Gets a value representing the <see cref="Scp244Pickup"/> being opened.
        /// </summary>
        public Scp244Pickup Pickup { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can open SCP-244.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
