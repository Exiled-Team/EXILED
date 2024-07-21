// -----------------------------------------------------------------------
// <copyright file="Usable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="UsableItem"/>.
    /// </summary>
    public class Usable : Item, IWrapper<UsableItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Usable"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="UsableItem"/> class.</param>
        public Usable(UsableItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Usable"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the usable item.</param>
        internal Usable(ItemType type)
            : this((UsableItem)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="UsableItem"/> that this class is encapsulating.
        /// </summary>
        public new UsableItem Base { get; }

        /// <summary>
        /// Gets a value indicating whether this item is equippable.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Usable))]
        public bool Equippable => Base.AllowEquip;

        /// <summary>
        /// Gets a value indicating whether this item is holsterable.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Usable))]
        public bool Holsterable => Base.AllowHolster;

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        [EProperty(category: nameof(Usable))]
        public new float Weight
        {
            get => Base._weight;
            set => Base._weight = value;
        }

        /// <summary>
        /// Gets a value indicating whether the item is currently being used.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Usable))]
        public bool IsUsing => Base.IsUsing;

        /// <summary>
        /// Gets or sets how long it takes to use the item.
        /// </summary>
        [EProperty(category: nameof(Usable))]
        public float UseTime
        {
            get => Base.UseTime;
            set => Base.UseTime = value;
        }

        /// <summary>
        /// Gets or sets how long after using starts a player has to cancel using the item.
        /// </summary>
        [EProperty(category: nameof(Usable))]
        public float MaxCancellableTime
        {
            get => Base.MaxCancellableTime;
            set => Base.MaxCancellableTime = value;
        }

        /// <summary>
        /// Gets or sets the cooldown between repeated uses of this item.
        /// </summary>
        [EProperty(category: nameof(Usable))]
        public float RemainingCooldown
        {
            get => UsableItemsController.GlobalItemCooldowns.TryGetValue(Serial, out float value) ? value : -1;
            set => UsableItemsController.GlobalItemCooldowns[Serial] = Time.timeSinceLevelLoad + value;
        }

        /// <summary>
        /// Gets all the cooldown between uses of this item.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Usable))]
        public float PlayerGetCooldown => UsableItemsController.GetCooldown(Serial, Base, UsableItemsController.GetHandler(Base.Owner));

        /// <summary>
        /// Creates the <see cref="Pickup"/> that based on this <see cref="Item"/>.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="spawn">Whether the <see cref="Pickup"/> should be initially spawned.</param>
        /// <returns>The created <see cref="Pickup"/>.</returns>
        public override Pickup CreatePickup(Vector3 position, Quaternion rotation = default, bool spawn = true)
        {
            PickupSyncInfo info = new(Type, Weight, Serial);

            ItemPickupBase ipb = InventoryExtensions.ServerCreatePickup(Base, info, position, rotation);

            Base.OnRemoved(ipb);

            Pickup pickup = Pickup.Get(ipb);

            if (spawn)
                pickup.Spawn();

            return pickup;
        }

        /// <summary>
        /// Uses the item.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The <see cref="Item.Owner"/> of the item cannot be <see langword="null"/>.</exception>
        public virtual void Use()
        {
            if (Owner is null)
                throw new System.InvalidOperationException("The Owner of the item cannot be null.");

            Owner.UseItem(this);
        }

        /// <inheritdoc/>
        internal override void ReadPickupInfo(Pickup pickup)
        {
            base.ReadPickupInfo(pickup);
            if (pickup is UsablePickup usablePickup)
            {
                UseTime = usablePickup.UseTime;
                MaxCancellableTime = usablePickup.MaxCancellableTime;
            }
        }
    }
}