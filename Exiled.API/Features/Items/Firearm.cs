// -----------------------------------------------------------------------
// <copyright file="Firearm.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using System;

    using CameraShaking;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.Firearms.Firearm"/>.
    /// </summary>
    public class Firearm : Item {
        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Firearm(InventorySystem.Items.Firearms.Firearm itemBase)
            : base(itemBase) {
            Base = itemBase;

            switch (Base) {
                case AutomaticFirearm auto:
                    Base.AmmoManagerModule =
                        new AutomaticAmmoManager(auto, auto._baseMaxAmmo, 1, auto._boltTravelTime == 0);
                    break;
                case Shotgun shotgun:
                    Base.AmmoManagerModule = new TubularMagazineAmmoManager(shotgun, Serial, shotgun._ammoCapacity, shotgun._numberOfChambers, 0.5f, 3, "ShellsToLoad", ActionName.Zoom, ActionName.Shoot);
                    break;
                default:
                    Base.AmmoManagerModule = new ClipLoadedInternalMagAmmoManager(Base, 6);
                    break;
            }

            Base._status = new FirearmStatus(MaxAmmo, FirearmStatusFlags.MagazineInserted, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Item.Type"/></param>
        public Firearm(ItemType type)
            : this((InventorySystem.Items.Firearms.Firearm)Server.Host.Inventory.CreateItemInstance(type, false)) {
        }

        /// <inheritdoc cref="Item.Base"/>
        public new InventorySystem.Items.Firearms.Firearm Base { get; }

        /// <summary>
        /// Gets or sets the amount of ammo in the firearm.
        /// </summary>
        public byte Ammo {
            get => Base.Status.Ammo;
            set => Base.Status = new FirearmStatus(value, Base.Status.Flags, Base.Status.Attachments);
        }

        /// <summary>
        /// Gets the max ammo for this firearm.
        /// </summary>
        public byte MaxAmmo => Base.AmmoManagerModule.MaxAmmo;

        /// <summary>
        /// Gets the <see cref="AmmoType"/> of the firearm.
        /// </summary>
        public AmmoType AmmoType => Base.AmmoType.GetAmmoType();

        /// <summary>
        /// Gets a value indicating whether the firearm is being aimed.
        /// </summary>
        public bool Aiming => Base.AdsModule.ServerAds;

        /// <summary>
        /// Gets a value indicating whether the firearm's flashlight module is enabled.
        /// </summary>
        public bool FlashlightEnabled => Base.Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled);

        /// <summary>
        /// Gets or sets the <see cref="FirearmAttachment"/>s of the firearm.
        /// </summary>
        public FirearmAttachment[] Attachments {
            get => Base.Attachments;
            set => Base.Attachments = value;
        }

        /// <summary>
        /// Gets or sets the fire rate of the firearm, if it is an automatic weapon.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set this value for a weapon that is semi-automatic.</exception>
        public float FireRate {
            get => Base is AutomaticFirearm auto ? auto._fireRate : 1f;
            set {
                if (Base is AutomaticFirearm auto)
                    auto._fireRate = value;
                else
                    throw new InvalidOperationException("You cannot change the firerate of non-automatic weapons.");
            }
        }

        /// <summary>
        /// Gets or sets the recoil settings of the firearm, if it's an automatic weapon.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set this value for a weapon that is semi-automatic.</exception>
        public RecoilSettings Recoil {
            get => Base is AutomaticFirearm auto ? auto._recoil : default;
            set {
                if (Base is AutomaticFirearm auto)
                    auto.ActionModule = new AutomaticAction(Base, auto._semiAutomatic, auto._boltTravelTime, 1f / auto._fireRate, auto._dryfireClipId, auto._triggerClipId, auto._gunshotPitchRandomization, value, auto._recoilPattern, false);
                else
                    throw new InvalidOperationException("You cannot change the recoil pattern of non-automatic weapons.");
            }
        }
    }
}
