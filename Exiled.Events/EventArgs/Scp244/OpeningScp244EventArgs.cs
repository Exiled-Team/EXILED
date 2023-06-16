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
    ///     Contains all information before a player picks up an SCP-244.
    /// </summary>
    public class OpeningScp244EventArgs : IPickupEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OpeningScp244EventArgs" /> class.
        /// </summary>
        /// <param name="pickup">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        public OpeningScp244EventArgs(Scp244DeployablePickup pickup)
        {
            Scp244 = (Scp244Pickup)API.Features.Pickups.Pickup.Get(pickup);
        }

        /// <summary>
        /// Gets a value representing the <see cref="Scp244Pickup"/> being picked up.
        /// </summary>
        public Scp244Pickup Scp244 { get; }

        /// <inheritdoc />
        public Pickup Pickup => Scp244;

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}