// -----------------------------------------------------------------------
// <copyright file="FirearmPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;
    using InventorySystem.Items;

    using BaseFirearm = InventorySystem.Items.Firearms.FirearmPickup;
    using FirearmItem = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// A wrapper class for a Firearm pickup.
    /// </summary>
    public class FirearmPickup : Pickup, IWrapper<BaseFirearm>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseFirearm"/> class.</param>
        internal FirearmPickup(BaseFirearm pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal FirearmPickup(ItemType type)
            : base(type)
        {
            Base = (BaseFirearm)((Pickup)this).Base;
            Base.OnDistributed();

            if (type is ItemType.ParticleDisruptor && Status.Ammo <= 0)
                Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, 0);
        }

        /// <summary>
        /// Gets the <see cref="BaseFirearm"/> that this class is encapsulating.
        /// </summary>
        public new BaseFirearm Base { get; }

        /// <summary>
        /// Gets the <see cref="Enums.FirearmType"/> of the firearm.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(FirearmPickup))]
        public FirearmType FirearmType => Type.GetFirearmType();

        /// <summary>
        /// Gets or sets the <see cref="Enums.AmmoType"/> of the firearm.
        /// </summary>
        [EProperty(category: nameof(FirearmPickup))]
        public AmmoType AmmoType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FirearmStatus"/>.
        /// </summary>
        [EProperty(category: nameof(FirearmPickup))]
        public FirearmStatus Status
        {
            get => Base.NetworkStatus;
            set => Base.NetworkStatus = value;
        }

        /// <summary>
        /// Gets or sets a value indicating how many ammo have this <see cref="FirearmPickup"/>.
        /// </summary>
        [EProperty(category: nameof(FirearmPickup))]
        public byte Ammo
        {
            get => Base.NetworkStatus.Ammo;
            set => Base.NetworkStatus = new(value, Base.NetworkStatus.Flags, Base.NetworkStatus.Attachments);
        }

        /// <summary>
        /// Gets or sets the max ammo the Firearm can have.
        /// </summary>
        [EProperty(category: nameof(FirearmPickup))]
        public byte MaxAmmo { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FirearmStatusFlags"/>.
        /// </summary>
        [EProperty(category: nameof(FirearmPickup))]
        public FirearmStatusFlags Flags
        {
            get => Base.NetworkStatus.Flags;
            set => Base.NetworkStatus = new(Base.NetworkStatus.Ammo, value, Base.NetworkStatus.Attachments);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attachment code have this <see cref="FirearmPickup"/>.
        /// </summary>
        [EProperty(category: nameof(FirearmPickup))]
        public uint Attachments
        {
            get => Base.NetworkStatus.Attachments;
            set => Base.NetworkStatus = new(Base.NetworkStatus.Ammo, Base.NetworkStatus.Flags, value);
        }

        /// <summary>
        /// Returns the FirearmPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing FirearmPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{IsDistributed}| -{Ammo}-";

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);

            if (item is Items.Firearm firearm)
            {
                MaxAmmo = firearm.MaxAmmo;
                AmmoType = firearm.AmmoType;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);

            if (itemBase is FirearmItem firearm)
            {
                MaxAmmo = firearm switch
                {
                    AutomaticFirearm autoFirearm => autoFirearm._baseMaxAmmo,
                    Revolver => 6,
                    Shotgun shotgun => shotgun._ammoCapacity,
                    _ => 0
                };
                AmmoType = firearm is AutomaticFirearm automaticFirearm ? automaticFirearm._ammoType.GetAmmoType() : firearm.ItemTypeId.GetAmmoType();
            }
        }
    }
}