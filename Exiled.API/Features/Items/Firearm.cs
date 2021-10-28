// -----------------------------------------------------------------------
// <copyright file="Firearm.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CameraShaking;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Structs;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.Firearms.Firearm"/>.
    /// </summary>
    public class Firearm : Item
    {
        private static readonly Dictionary<ItemType, BaseCode> FirearmPairs = new Dictionary<ItemType, BaseCode>()
        {
            { ItemType.GunCOM15, BaseCode.GunCOM15 },
            { ItemType.GunCOM18, BaseCode.GunCOM18 },
            { ItemType.GunRevolver, BaseCode.GunRevolver },
            { ItemType.GunE11SR, BaseCode.GunE11SR },
            { ItemType.GunCrossvec, BaseCode.GunCrossvec },
            { ItemType.GunFSP9, BaseCode.GunFSP9 },
            { ItemType.GunLogicer, BaseCode.GunLogicer },
            { ItemType.GunAK, BaseCode.GunAK },
            { ItemType.GunShotgun, BaseCode.GunShotgun },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Firearm(InventorySystem.Items.Firearms.Firearm itemBase)
            : base(itemBase)
        {
            Base = itemBase;

            switch (Base)
            {
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
            : this((InventorySystem.Items.Firearms.Firearm)Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }

        /// <summary>
        /// Gets all available attachments for all firearms.
        /// </summary>
        public static Dictionary<ItemType, AttachmentIdentifier[]> AvailableAttachments => new Dictionary<ItemType, AttachmentIdentifier[]>()
        {
            {
                ItemType.GunCOM15,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.StandardBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.SoundSuppressor, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.StandardMagFMJ, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.ExtendedMagFMJ, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.None, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.Flashlight, AttachmentSlot.BottomRail),
                }
            },
            {
                ItemType.GunCOM18,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.DotSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.SoundSuppressor, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.StandardBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.ExtendedBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.None, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.Laser, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.Flashlight, AttachmentSlot.BottomRail),
                }
            },
            {
                ItemType.GunRevolver,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.StandardBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.ExtendedBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.ShortBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.None, AttachmentSlot.Stock),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.HeavyStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.CylinderMag6, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.CylinderMag4, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.CylinderMag8, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.DotSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(512, AttachmentNameTranslation.ScopeSight, AttachmentSlot.Sight),
                }
            },
            {
                ItemType.GunE11SR,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.HoloSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.DotSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.NightVisionSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.ScopeSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.StandardMagFMJ, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.ExtendedMagFMJ, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.LowcapMagJHP, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.LowcapMagAP, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(512, AttachmentNameTranslation.RifleBody, AttachmentSlot.Body),
                    new AttachmentIdentifier(1024, AttachmentNameTranslation.CarbineBody, AttachmentSlot.Body),
                    new AttachmentIdentifier(2048, AttachmentNameTranslation.StandardStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(4096, AttachmentNameTranslation.LightweightStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(8192, AttachmentNameTranslation.RecoilReducingStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(16384, AttachmentNameTranslation.None, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(32768, AttachmentNameTranslation.SoundSuppressor, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(65536, AttachmentNameTranslation.FlashHider, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(131072, AttachmentNameTranslation.MuzzleBooster, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(262144, AttachmentNameTranslation.MuzzleBrake, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(524288, AttachmentNameTranslation.None, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(1048576, AttachmentNameTranslation.Foregrip, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(2097152, AttachmentNameTranslation.Laser, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(4194304, AttachmentNameTranslation.None, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(8388608, AttachmentNameTranslation.Flashlight, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(16777216, AttachmentNameTranslation.AmmoCounter, AttachmentSlot.SideRail),
                }
            },
            {
                ItemType.GunCrossvec,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.HoloSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.DotSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.NightVisionSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.ExtendedStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.RetractedStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.None, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.Foregrip, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.Laser, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(512, AttachmentNameTranslation.Flashlight, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(1024, AttachmentNameTranslation.StandardBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(2048, AttachmentNameTranslation.SoundSuppressor, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(4096, AttachmentNameTranslation.ExtendedBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(8192, AttachmentNameTranslation.FlashHider, AttachmentSlot.Barrel),
                }
            },
            {
                ItemType.GunFSP9,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.HoloSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.DotSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.None, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.FlashHider, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.SoundSuppressor, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.None, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.AmmoCounter, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.Laser, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(512, AttachmentNameTranslation.Flashlight, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(1024, AttachmentNameTranslation.None, AttachmentSlot.Stability),
                    new AttachmentIdentifier(2048, AttachmentNameTranslation.Foregrip, AttachmentSlot.Stability),
                    new AttachmentIdentifier(4096, AttachmentNameTranslation.RetractedStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(8192, AttachmentNameTranslation.ExtendedStock, AttachmentSlot.Stock),
                }
            },
            {
                ItemType.GunAK,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.HoloSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.ScopeSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.AmmoSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.StandardBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.ExtendedBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.SoundSuppressor, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.MuzzleBrake, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.MuzzleBooster, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(512, AttachmentNameTranslation.StandardStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(1024, AttachmentNameTranslation.HeavyStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(2048, AttachmentNameTranslation.NoRifleStock, AttachmentSlot.Stock),
                    new AttachmentIdentifier(4096, AttachmentNameTranslation.StandardMagAP, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(8192, AttachmentNameTranslation.StandardMagJHP, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(16384, AttachmentNameTranslation.DrumMagAP, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(32768, AttachmentNameTranslation.DrumMagJHP, AttachmentSlot.Ammunition),
                    new AttachmentIdentifier(65536, AttachmentNameTranslation.None, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(131072, AttachmentNameTranslation.Foregrip, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(262144, AttachmentNameTranslation.Laser, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(524288, AttachmentNameTranslation.Flashlight, AttachmentSlot.SideRail),
                }
            },
            {
                ItemType.GunLogicer,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.DotSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.AmmoSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.NightVisionSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.StandardBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.FlashHider, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.MuzzleBrake, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.ShortBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.None, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(512, AttachmentNameTranslation.Foregrip, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(1024, AttachmentNameTranslation.Flashlight, AttachmentSlot.BottomRail),
                    new AttachmentIdentifier(2048, AttachmentNameTranslation.Laser, AttachmentSlot.BottomRail),
                }
            },
            {
                ItemType.GunShotgun,
                new AttachmentIdentifier[]
                {
                    new AttachmentIdentifier(1, AttachmentNameTranslation.IronSights, AttachmentSlot.Sight),
                    new AttachmentIdentifier(2, AttachmentNameTranslation.HoloSight, AttachmentSlot.Sight),
                    new AttachmentIdentifier(4, AttachmentNameTranslation.None, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(8, AttachmentNameTranslation.ShotgunChoke, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(16, AttachmentNameTranslation.ShotgunExtendedBarrel, AttachmentSlot.Barrel),
                    new AttachmentIdentifier(32, AttachmentNameTranslation.None, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(64, AttachmentNameTranslation.AmmoCounter, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(128, AttachmentNameTranslation.Laser, AttachmentSlot.SideRail),
                    new AttachmentIdentifier(256, AttachmentNameTranslation.Flashlight, AttachmentSlot.SideRail),
                }
            },
        };

        /// <inheritdoc cref="Item.Base"/>
        public new InventorySystem.Items.Firearms.Firearm Base { get; }

        /// <summary>
        /// Gets or sets the amount of ammo in the firearm.
        /// </summary>
        public byte Ammo
        {
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
        /// Gets the <see cref="DamageTypes.DamageType"/> of the firearm.
        /// </summary>
        public DamageTypes.DamageType DamageType => Base.DamageType;

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
        public FirearmAttachment[] Attachments
        {
            get => Base.Attachments;
            set => Base.Attachments = value;
        }

        /// <summary>
        /// Gets the <see cref="Enums.BaseCode"/> of the firearm.
        /// </summary>
        public BaseCode BaseCode => FirearmPairs[Type];

        /// <summary>
        /// Gets or sets the fire rate of the firearm, if it is an automatic weapon.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set this value for a weapon that is semi-automatic.</exception>
        public float FireRate
        {
            get => Base is AutomaticFirearm auto ? auto._fireRate : 1f;
            set
            {
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
        public RecoilSettings Recoil
        {
            get => Base is AutomaticFirearm auto ? auto._recoil : default;
            set
            {
                if (Base is AutomaticFirearm auto)
                    auto.ActionModule = new AutomaticAction(Base, auto._semiAutomatic, auto._boltTravelTime, 1f / auto._fireRate, auto._dryfireClipId, auto._triggerClipId, auto._gunshotPitchRandomization, value, auto._recoilPattern, false);
                else
                    throw new InvalidOperationException("You cannot change the recoil pattern of non-automatic weapons.");
            }
        }

        /// <summary>
        /// Adds an <see cref="AttachmentIdentifier"/> to the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to add.</param>
        public void AddAttachment(AttachmentIdentifier identifier)
        {
            foreach (FirearmAttachment attachment in Attachments)
            {
                if (identifier.Slot != attachment.Slot)
                    continue;

                Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() + identifier.Code, true);
            }
        }

        /// <summary>
        /// Removes an <see cref="AttachmentIdentifier"/> from the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to remove.</param>
        public void RemoveAttachment(AttachmentIdentifier identifier)
        {
            foreach (FirearmAttachment attachment in Attachments)
            {
                if (identifier.Slot != attachment.Slot || !attachment.IsEnabled)
                    continue;

                Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() - identifier.Code, true);
            }
        }

        /// <summary>
        /// Removes all attachments from the firearm.
        /// </summary>
        public void ClearAttachments() => Base.ApplyAttachmentsCode((uint)BaseCode, true);

        /// <summary>
        /// Tries to get a <see cref="FirearmAttachment"/> from the specified <see cref="Firearm"/>'s <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to check.</param>
        /// <param name="firearmAttachment">The corresponding <see cref="FirearmAttachment"/>.</param>
        /// <returns>A value indicating whether or not the firearm has the specified <see cref="FirearmAttachment"/>.</returns>
        public bool TryGetAttachment(AttachmentIdentifier identifier, out FirearmAttachment firearmAttachment)
        {
            firearmAttachment = default;

            if (!Attachments.Any(attachment => attachment.Name == identifier.Name && attachment.Slot == identifier.Slot))
                return false;

            firearmAttachment = Attachments.FirstOrDefault(attachment => attachment.Name == identifier.Name && attachment.Slot == identifier.Slot);

            return true;
        }
    }
}
