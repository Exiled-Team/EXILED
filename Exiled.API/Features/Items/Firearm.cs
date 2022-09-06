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
    using Exiled.API.Features.Pickups;
    using Exiled.API.Structs;

    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;

    using UnityEngine;

    using BaseFirearm = InventorySystem.Items.Firearms.Firearm;
    using FirearmPickup = Exiled.API.Features.Pickups.FirearmPickup;
    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.Firearms.Firearm"/>.
    /// </summary>
    public class Firearm : Item
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Firearm"/> which contains all the existing firearms based on all the <see cref="ItemType"/>s.
        /// </summary>
        internal static readonly List<Firearm> FirearmInstances = new();

        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> which contains all pairs for <see cref="ItemType"/> and <see cref="Enums.BaseCode"/>.
        /// </summary>
        internal static readonly IReadOnlyDictionary<ItemType, BaseCode> FirearmPairs = new Dictionary<ItemType, BaseCode>()
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
            { ItemType.ParticleDisruptor, BaseCode.Disruptor },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="InventorySystem.Items.Firearms.Firearm"/> class.</param>
        public Firearm(BaseFirearm itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the firearm.</param>
        internal Firearm(ItemType type)
            : this((BaseFirearm)Server.Host.Inventory.CreateItemInstance(type, false))
        {
            FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;
            if (Base.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
                firearmStatusFlags |= FirearmStatusFlags.FlashlightEnabled;

            Base.Status = new FirearmStatus(MaxAmmo, firearmStatusFlags, Base.Status.Attachments);
        }

        /// <inheritdoc cref="AvailableAttachmentsValue"/>.
        public static IReadOnlyDictionary<ItemType, AttachmentIdentifier[]> AvailableAttachments => AvailableAttachmentsValue;

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.Firearms.Firearm"/> that this class is encapsulating.
        /// </summary>
        public new BaseFirearm Base { get; }

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
        /// Gets the <see cref="Enums.AmmoType"/> of the firearm.
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
        /// Gets the <see cref="Attachment"/>s of the firearm.
        /// </summary>
        public Attachment[] Attachments => Base.Attachments;

        /// <summary>
        /// Gets the <see cref="AttachmentIdentifier"/>s of the firearm.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> AttachmentIdentifiers => this.GetAttachmentIdentifiers();

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
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="AttachmentIdentifier"/>[] which contains all available attachments for all firearms.
        /// </summary>
        internal static Dictionary<ItemType, AttachmentIdentifier[]> AvailableAttachmentsValue { get; } = new();

        /// <summary>
        /// Adds a <see cref="AttachmentIdentifier"/> to the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to add.</param>
        public void AddAttachment(AttachmentIdentifier identifier)
        {
            uint toRemove = 0;
            uint code = 1;
            foreach (Attachment attachment in Base.Attachments)
            {
                if (attachment.Slot == identifier.Slot && attachment.IsEnabled)
                {
                    toRemove = code;
                    break;
                }

                code *= 2;
            }

            uint newCode = identifier.Code == 0 ?
                AvailableAttachments[Type].FirstOrDefault(attId =>
                attId.Name == identifier.Name).Code :
                identifier.Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~toRemove | newCode, true);

            Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Adds a <see cref="Attachment"/> of the specified <see cref="AttachmentName"/> to the firearm.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentName"/> to add.</param>
        public void AddAttachment(AttachmentName attachmentName) => AddAttachment(AttachmentIdentifier.Get(Type, attachmentName));

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to the firearm.
        /// </summary>
        /// <param name="identifiers">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to add.</param>
        public void AddAttachment(IEnumerable<AttachmentIdentifier> identifiers)
        {
            foreach (AttachmentIdentifier identifier in identifiers)
                AddAttachment(identifier);
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to the firearm.
        /// </summary>
        /// <param name="attachmentNames">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to add.</param>
        public void AddAttachment(IEnumerable<AttachmentName> attachmentNames)
        {
            foreach (AttachmentName attachmentName in attachmentNames)
                AddAttachment(attachmentName);
        }

        /// <summary>
        /// Removes a <see cref="AttachmentIdentifier"/> from the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to remove.</param>
        public void RemoveAttachment(AttachmentIdentifier identifier)
        {
            if (!Attachments.Any(attachment => attachment.Name == identifier.Name && attachment.IsEnabled))
                return;

            uint code = identifier.Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);

            if (identifier.Name == AttachmentName.Flashlight)
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags & ~FirearmStatusFlags.FlashlightEnabled, Base.GetCurrentAttachmentsCode());
            else
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Removes a <see cref="Attachment"/> of the specified <see cref="AttachmentName"/> from the firearm.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentName"/> to remove.</param>
        public void RemoveAttachment(AttachmentName attachmentName)
        {
            Attachment firearmAttachment = Attachments.FirstOrDefault(att => att.Name == attachmentName && att.IsEnabled);

            if (firearmAttachment is null)
                return;

            uint code = AttachmentIdentifier.Get(Type, attachmentName).Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);

            if (attachmentName == AttachmentName.Flashlight)
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags & ~FirearmStatusFlags.FlashlightEnabled, Base.GetCurrentAttachmentsCode());
            else
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Removes a <see cref="Attachment"/> of the specified <see cref="AttachmentSlot"/> from the firearm.
        /// </summary>
        /// <param name="attachmentSlot">The <see cref="AttachmentSlot"/> to remove.</param>
        public void RemoveAttachment(AttachmentSlot attachmentSlot)
        {
            Attachment firearmAttachment = Attachments.FirstOrDefault(att => att.Slot == attachmentSlot && att.IsEnabled);

            if (firearmAttachment is null)
                return;

            uint code = AvailableAttachments[Type].FirstOrDefault(attId => attId == firearmAttachment).Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);

            if (firearmAttachment.Name == AttachmentName.Flashlight)
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags & ~FirearmStatusFlags.FlashlightEnabled, Base.GetCurrentAttachmentsCode());
            else
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Removes a <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> from the firearm.
        /// </summary>
        /// <param name="identifiers">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentIdentifier> identifiers)
        {
            foreach (AttachmentIdentifier identifier in identifiers)
                RemoveAttachment(identifier);
        }

        /// <summary>
        /// Removes a list of <see cref="Attachment"/> of the specified <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> from the firearm.
        /// </summary>
        /// <param name="attachmentNames">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentName> attachmentNames)
        {
            foreach (AttachmentName attachmentName in attachmentNames)
                RemoveAttachment(attachmentName);
        }

        /// <summary>
        /// Removes a list of <see cref="Attachment"/> of the specified <see cref="IEnumerable{T}"/> of <see cref="AttachmentSlot"/> from the firearm.
        /// </summary>
        /// <param name="attachmentSlots">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentSlot"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentSlot> attachmentSlots)
        {
            foreach (AttachmentSlot attachmentSlot in attachmentSlots)
                RemoveAttachment(attachmentSlot);
        }

        /// <summary>
        /// Removes all attachments from the firearm.
        /// </summary>
        public void ClearAttachments() => Base.ApplyAttachmentsCode((uint)BaseCode, true);

        /// <summary>
        /// Gets a <see cref="Attachment"/> of the specified <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to check.</param>
        /// <returns>The corresponding <see cref="Attachment"/>.</returns>
        public Attachment GetAttachment(AttachmentIdentifier identifier) => Attachments.FirstOrDefault(attachment => attachment == identifier);

        /// <summary>
        /// Tries to get a <see cref="Attachment"/> of the specified <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to check.</param>
        /// <param name="firearmAttachment">The corresponding <see cref="Attachment"/>.</param>
        /// <returns>A value indicating whether or not the firearm has the specified <see cref="Attachment"/>.</returns>
        public bool TryGetAttachment(AttachmentIdentifier identifier, out Attachment firearmAttachment)
        {
            firearmAttachment = default;

            if (!Attachments.Any(attachment => attachment.Name == identifier.Name))
                return false;

            firearmAttachment = GetAttachment(identifier);

            return true;
        }

        /// <summary>
        /// Tries to get a <see cref="Attachment"/> of the specified <see cref="AttachmentName"/>.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentName"/> to check.</param>
        /// <param name="firearmAttachment">The corresponding <see cref="Attachment"/>.</param>
        /// <returns>A value indicating whether or not the firearm has the specified <see cref="Attachment"/>.</returns>
        public bool TryGetAttachment(AttachmentName attachmentName, out Attachment firearmAttachment)
        {
            firearmAttachment = default;

            if (Attachments.All(attachment => attachment.Name != attachmentName))
                return false;

            firearmAttachment = GetAttachment(AttachmentIdentifier.Get(Type, attachmentName));

            return true;
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
            FirearmPickup pickup = (FirearmPickup)Pickup.Get(Object.Instantiate(Base.PickupDropModel, position, rotation));

            pickup.Info = new()
            {
                ItemId = Type,
                Position = position,
                Weight = pickup.Weight,
                Serial = ItemSerialGenerator.GenerateNext(),
                Rotation = new LowPrecisionQuaternion(rotation),
            };

            pickup.Scale = Scale;
            pickup.Status = Base.Status;

            if (spawn)
                pickup.Spawn();

            return pickup;
        }

        /// <summary>
        /// Clones current <see cref="Firearm"/> object.
        /// </summary>
        /// <returns> New <see cref="Firearm"/> object. </returns>
        public override Item Clone()
        {
            Firearm cloneableItem = new(Type)
            {
                Ammo = Ammo,
            };

            if (cloneableItem.Base is AutomaticFirearm)
            {
                cloneableItem.FireRate = FireRate;
                cloneableItem.Recoil = Recoil;
            }

            cloneableItem.AddAttachment(AttachmentIdentifiers);

            return cloneableItem;
        }

        /// <summary>
        /// Change the owner of the <see cref="Firearm"/>.
        /// </summary>
        /// <param name="oldOwner">old <see cref="Firearm"/> owner.</param>
        /// <param name="newOwner">new <see cref="Firearm"/> owner.</param>
        internal override void ChangeOwner(Player oldOwner, Player newOwner)
        {
            Base.Owner = newOwner.ReferenceHub;

            Base.HitregModule = Base switch
            {
                AutomaticFirearm automaticFirearm =>
                    new SingleBulletHitreg(automaticFirearm, automaticFirearm.Owner, automaticFirearm._recoilPattern),
                Shotgun shotgun =>
                    new BuckshotHitreg(shotgun, shotgun.Owner, shotgun._buckshotStats),
                ParticleDisruptor particleDisruptor =>
                    new DisruptorHitreg(particleDisruptor, particleDisruptor.Owner, particleDisruptor._explosionSettings),
                Revolver revolver =>
                    new SingleBulletHitreg(revolver, revolver.Owner),
                _ => throw new NotImplementedException("Should never happend"),
            };

            Base._sendStatusNextFrame = true;
            Base._footprintValid = false;
        }
    }
}
