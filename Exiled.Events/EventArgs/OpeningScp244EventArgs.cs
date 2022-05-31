// -----------------------------------------------------------------------
// <copyright file="OpeningScp244EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Usables.Scp244;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player picks up an SCP-244.
    /// </summary>
    public class OpeningScp244EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningScp244EventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="Scp244"/></param>
        public OpeningScp244EventArgs(Scp244DeployablePickup pickup)
        {
            Scp244 = pickup;
            Pickup = Pickup.Get(pickup);
        }

        /// <summary>
        /// Gets a value representing the <see cref="Scp244DeployablePickup"/> being picked up.
        /// </summary>
        public Scp244DeployablePickup Scp244 { get; }

        /// <summary>
        /// Gets a value representing the <see cref="Pickup"/> being picked up.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
