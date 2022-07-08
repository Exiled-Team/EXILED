// -----------------------------------------------------------------------
// <copyright file="Scp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Features.Pickups;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for SCP-244.
    /// </summary>
    public class Scp244 : Usable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp244Item"/> class.</param>
        public Scp244(Scp244Item itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244"/> class, as well as a new SCP-244 item.
        /// </summary>
        /// <param name="scp244Type">The type of SCP-244, either <see cref="ItemType.SCP244a"/> or <see cref="ItemType.SCP244b"/>.</param>
        internal Scp244(ItemType scp244Type)
            : this((Scp244Item)Server.Host.Inventory.CreateItemInstance(scp244Type, false))
        {
        }

        /// <summary>
        /// Gets the <see cref="Scp244Item"/> that this class is encapsulating.
        /// </summary>
        public new Scp244Item Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-244 will spawn primed.
        /// </summary>
        public bool Primed
        {
            get => Base._primed;
            set => Base._primed = value;
        }

        /// <summary>
        /// Creates the <see cref="Pickup"/> that based on this <see cref="Item"/>.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="spawn">Whether the <see cref="Pickup"/> should be initially spawned.</param>
        /// <returns>The created <see cref="Pickup"/>.</returns>
        public override Pickup CreatePickup(Vector3 position, Quaternion rotation = default, bool spawn = true)
        {
            Scp244DeployablePickup ipb = (Scp244DeployablePickup)Object.Instantiate(Base.PickupDropModel, position, rotation);

            PickupSyncInfo info = new()
            {
                ItemId = Type,
                Position = position,
                Weight = Weight,
                Rotation = new LowPrecisionQuaternion(rotation),
            };

            ipb.NetworkInfo = info;
            ipb.InfoReceived(default, info);

            ipb.State = Base._primed ? Scp244State.Active : Scp244State.Idle;

            ipb.transform.localScale = Scale;

            Pickup pickup = Pickup.Get(ipb);

            if (spawn)
                pickup.Spawn();

            return pickup;
        }

        /// <summary>
        /// Returns the SCP-244 in a human readable format.
        /// </summary>
        /// <returns>A string containing SCP-244 related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* -{Primed}-";
    }
}
