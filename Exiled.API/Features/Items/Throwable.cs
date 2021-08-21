// -----------------------------------------------------------------------
// <copyright file="Throwable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Enums;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for throwable items.
    /// </summary>
    public class Throwable : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Throwable(ThrowableItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Base"/></param>
        public Throwable(ItemType type)
            : this((ThrowableItem)Server.Host.Inventory.CreateItemInstance((global::ItemType)type, false))
        {
        }

        /// <summary>
        /// Gets the <see cref="ThrowableItem"/> base for this item.
        /// </summary>
        public new ThrowableItem Base { get; }

        /// <summary>
        /// Gets the <see cref="ThrownProjectile"/> for this item.
        /// </summary>
        public ThrownProjectile Projectile => Base.Projectile;

        /// <summary>
        /// Gets or sets the amount of time it takes to pull the pin.
        /// </summary>
        public float PinPullTime
        {
            get => Base._pinPullTime;
            set => Base._pinPullTime = value;
        }
    }
}
