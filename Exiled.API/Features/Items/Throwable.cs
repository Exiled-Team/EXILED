// -----------------------------------------------------------------------
// <copyright file="Throwable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using Exiled.API.Enums;

    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for throwable items.
    /// </summary>
    public class Throwable : Item {
        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Throwable(ThrowableItem itemBase)
            : base(itemBase) {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Base"/></param>
        /// <param name="player"><inheritdoc cref="Item.Owner"/></param>
        /// <remarks>The player parameter will always need to be defined if this throwable is custom using Exiled.CustomItems.</remarks>
        public Throwable(ItemType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true)) {
        }

        /// <summary>
        /// Gets the <see cref="ThrowableItem"/> base for this item.
        /// </summary>
        public new ThrowableItem Base { get; internal set; }

        /// <summary>
        /// Gets or sets the amount of time it takes to pull the pin.
        /// </summary>
        public float PinPullTime {
            get => Base._pinPullTime;
            set => Base._pinPullTime = value;
        }

        /// <summary>
        /// Throws the item.
        /// </summary>
        /// <param name="fullForce">Whether to use full or half force.</param>
        public void Throw(bool fullForce = true) => Base.ServerThrow(fullForce, ThrowableNetworkHandler.GetLimitedVelocity(Base.Owner.playerMovementSync.PlayerVelocity));

        /// <inheritdoc/>
        public override string ToString() {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}* |{PinPullTime}|";
        }
    }
}
