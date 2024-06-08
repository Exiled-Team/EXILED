// -----------------------------------------------------------------------
// <copyright file="ItemExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Features.Items;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using Structs;

    /// <summary>
    /// A set of extensions for <see cref="ItemType"/>.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is an ammo.
        /// </summary>
        /// <param name="item">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is an ammo or not.</returns>
        public static bool IsAmmo(this ItemType item) => item.GetAmmoType() is not AmmoType.None;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a weapon.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <param name="checkMicro">Indicates whether the MicroHID item should be taken into account or not.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a weapon or not.</returns>
        public static bool IsWeapon(this ItemType type, bool checkMicro = true) => type.GetFirearmType() is not FirearmType.None || (checkMicro && type is ItemType.MicroHID);

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is an SCP.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether or not the <see cref="ItemType"/> is an SCP.</returns>
        public static bool IsScp(this ItemType type) => GetCategory(type) == ItemCategory.SCPItem;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a throwable item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether or not the <see cref="ItemType"/> is a throwable item.</returns>
        public static bool IsThrowable(this ItemType type) => GetItemBase<ThrowableItem>(type) != null;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a medical item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether or not the <see cref="ItemType"/> is a medical item.</returns>
        public static bool IsMedical(this ItemType type) => GetCategory(type) == ItemCategory.Medical;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a utility item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether or not the <see cref="ItemType"/> is an utilty item.</returns>
        public static bool IsUtility(this ItemType type) => type is ItemType.Flashlight or ItemType.Radio;

        /// <summary>
        /// Check if a <see cref="ItemType"/> is an armor item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether or not the <see cref="ItemType"/> is an armor.</returns>
        public static bool IsArmor(this ItemType type) => GetCategory(type) == ItemCategory.Armor;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a keycard.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether or not the <see cref="ItemType"/> is a keycard.</returns>
        public static bool IsKeycard(this ItemType type) => GetCategory(type) == ItemCategory.Keycard;

        /// <summary>
        /// Given an <see cref="ItemType"/>, returns the matching <see cref="ItemBase"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/>.</param>
        /// <returns>The <see cref="ItemBase"/>, or <see langword="null"/> if not found.</returns>
        public static ItemBase GetItemBase(this ItemType type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return null;

            return itemBase;
        }

        /// <summary>
        /// Given an <see cref="ItemType"/>, returns the matching <see cref="ItemPickupBase"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/>.</param>
        /// <returns>The <see cref="ItemPickupBase"/>, or <see langword="null"/> if not found.</returns>
        public static ItemPickupBase GetPickupBase(this ItemType type) => GetItemBase(type)?.PickupDropModel;

        /// <summary>
        /// Given an <see cref="ItemType"/>, returns the matching <see cref="ItemBase"/>, casted to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/>.</param>
        /// <typeparam name="T">The type to cast the <see cref="ItemBase"/> to.</typeparam>
        /// <returns>The <see cref="ItemBase"/> casted to <typeparamref name="T"/>, or <see langword="null"/> if not found or couldn't be casted.</returns>
        public static T GetItemBase<T>(this ItemType type)
            where T : ItemBase
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return null;

            return itemBase as T;
        }

        /// <summary>
        /// Gets the maximum ammo of a weapon.
        /// </summary>
        /// <param name="type">The <see cref="FirearmType">weapon</see> that you want to get maximum of.</param>
        /// <returns>Returns the maximum.</returns>
        public static byte GetMaxAmmo(this FirearmType type)
        {
            InventorySystem.Items.Firearms.Firearm firearm = GetItemBase<InventorySystem.Items.Firearms.Firearm>(type.GetItemType());
            return firearm is null ? (byte)0 : firearm.AmmoManagerModule.MaxAmmo;
        }

        /// <summary>
        /// Returns the <see cref="AmmoType"/> of the weapon is using.
        /// </summary>
        /// <param name="type">The <see cref="FirearmType"/> to convert.</param>
        /// <returns>The given weapon's AmmoType.</returns>
        public static AmmoType GetWeaponAmmoType(this FirearmType type) => type switch
        {
            FirearmType.Com15 or FirearmType.Com18 or FirearmType.Com45 or FirearmType.Crossvec or FirearmType.FSP9 => AmmoType.Nato9,
            FirearmType.E11SR or FirearmType.FRMG0 => AmmoType.Nato556,
            FirearmType.A7 or FirearmType.AK or FirearmType.Logicer => AmmoType.Nato762,
            FirearmType.Revolver => AmmoType.Ammo44Cal,
            FirearmType.Shotgun => AmmoType.Ammo12Gauge,
            _ => AmmoType.None,
        };

        /// <summary>
        /// Converts a valid ammo <see cref="ItemType"/> into an <see cref="AmmoType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to convert.</param>
        /// <returns>The ammo type of the given item type.</returns>
        public static AmmoType GetAmmoType(this ItemType type) => type switch
        {
            ItemType.Ammo9x19 => AmmoType.Nato9,
            ItemType.Ammo556x45 => AmmoType.Nato556,
            ItemType.Ammo762x39 => AmmoType.Nato762,
            ItemType.Ammo12gauge => AmmoType.Ammo12Gauge,
            ItemType.Ammo44cal => AmmoType.Ammo44Cal,
            _ => AmmoType.None,
        };

        /// <summary>
        /// Converts a valid firearm <see cref="ItemType"/> into a <see cref="FirearmType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to convert.</param>
        /// <returns>The firearm type of the given item.</returns>
        public static FirearmType GetFirearmType(this ItemType type) => type switch
        {
            ItemType.GunCOM15 => FirearmType.Com15,
            ItemType.GunCOM18 => FirearmType.Com18,
            ItemType.GunE11SR => FirearmType.E11SR,
            ItemType.GunCrossvec => FirearmType.Crossvec,
            ItemType.GunFSP9 => FirearmType.FSP9,
            ItemType.GunLogicer => FirearmType.Logicer,
            ItemType.GunRevolver => FirearmType.Revolver,
            ItemType.GunAK => FirearmType.AK,
            ItemType.GunA7 => FirearmType.A7,
            ItemType.GunShotgun => FirearmType.Shotgun,
            ItemType.GunCom45 => FirearmType.Com45,
            ItemType.GunFRMG0 => FirearmType.FRMG0,
            ItemType.ParticleDisruptor => FirearmType.ParticleDisruptor,
            _ => FirearmType.None,
        };

        /// <summary>
        /// Converts an <see cref="AmmoType"/> into it's corresponding <see cref="ItemType"/>.
        /// </summary>
        /// <param name="type">The <see cref="AmmoType"/> to convert.</param>
        /// <returns>The Item type of the specified ammo.</returns>
        public static ItemType GetItemType(this AmmoType type) => type switch
        {
            AmmoType.Nato556 => ItemType.Ammo556x45,
            AmmoType.Nato762 => ItemType.Ammo762x39,
            AmmoType.Nato9 => ItemType.Ammo9x19,
            AmmoType.Ammo12Gauge => ItemType.Ammo12gauge,
            AmmoType.Ammo44Cal => ItemType.Ammo44cal,
            _ => ItemType.None,
        };

        /// <summary>
        /// Converts a <see cref="FirearmType"/> into it's corresponding <see cref="ItemType"/>.
        /// </summary>
        /// <param name="type">The <see cref="FirearmType"/> to convert.</param>
        /// <returns>The Item type of the specified firearm.</returns>
        public static ItemType GetItemType(this FirearmType type) => type switch
        {
            FirearmType.Com15 => ItemType.GunCOM15,
            FirearmType.Com18 => ItemType.GunCOM18,
            FirearmType.E11SR => ItemType.GunE11SR,
            FirearmType.Crossvec => ItemType.GunCrossvec,
            FirearmType.FSP9 => ItemType.GunFSP9,
            FirearmType.Logicer => ItemType.GunLogicer,
            FirearmType.Revolver => ItemType.GunRevolver,
            FirearmType.AK => ItemType.GunAK,
            FirearmType.A7 => ItemType.GunA7,
            FirearmType.Shotgun => ItemType.GunShotgun,
            FirearmType.Com45 => ItemType.GunCom45,
            FirearmType.FRMG0 => ItemType.GunFRMG0,
            FirearmType.ParticleDisruptor => ItemType.ParticleDisruptor,
            _ => ItemType.None,
        };

        /// <summary>
        /// Converts a valid projectile <see cref="ItemType"/> into a <see cref="ProjectileType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to convert.</param>
        /// <returns>The projectile type of the given item type, or <see cref="ProjectileType.None"/> if the provided item type is not a projectile.</returns>
        public static ProjectileType GetProjectileType(this ItemType type) => type switch
        {
            ItemType.GrenadeFlash => ProjectileType.Flashbang,
            ItemType.GrenadeHE => ProjectileType.FragGrenade,
            ItemType.SCP018 => ProjectileType.Scp018,
            ItemType.SCP2176 => ProjectileType.Scp2176,
            _ => ProjectileType.None,
        };

        /// <summary>
        /// Converts a <see cref="ProjectileType"/> into the corresponding <see cref="ItemType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ProjectileType"/> to convert.</param>
        /// <returns>The Item type of the specified grenade.</returns>
        public static ItemType GetItemType(this ProjectileType type) => type switch
        {
            ProjectileType.Flashbang => ItemType.GrenadeFlash,
            ProjectileType.Scp018 => ItemType.SCP018,
            ProjectileType.FragGrenade => ItemType.GrenadeHE,
            ProjectileType.Scp2176 => ItemType.SCP2176,
            _ => ItemType.None,
        };

        /// <summary>
        /// Converts a <see cref="IEnumerable{T}"/> of <see cref="Item"/>s into the corresponding <see cref="IEnumerable{T}"/> of <see cref="ItemType"/>s.
        /// </summary>
        /// <param name="items">The items to convert.</param>
        /// <returns>A new <see cref="List{T}"/> of <see cref="ItemType"/>s.</returns>
        public static IEnumerable<ItemType> GetItemTypes(this IEnumerable<Item> items) => items.Select(item => item.Type);

        /// <summary>
        /// Gets all <see cref="AttachmentIdentifier"/>s present on a <see cref="FirearmType"/>.
        /// </summary>
        /// <param name="type">The <see cref="FirearmType"/> to iterate over.</param>
        /// <param name="code">The <see cref="uint"/> value which represents the attachments code to check.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> value which represents all the attachments present on the specified <see cref="ItemType"/>.</returns>
        public static IEnumerable<AttachmentIdentifier> GetAttachmentIdentifiers(this FirearmType type, uint code)
        {
            if (type.GetBaseCode() > code)
                code = type.GetBaseCode();

            if (!Firearm.ItemTypeToFirearmInstance.TryGetValue(type, out Firearm firearm))
                throw new ArgumentException($"Couldn't find a Firearm instance matching the ItemType value: {type}.");

            firearm.Base.ApplyAttachmentsCode(code, true);

            return firearm.AttachmentIdentifiers;
        }

        /// <summary>
        /// Tries to get all <see cref="AttachmentIdentifier"/>s present on a <see cref="FirearmType"/>.
        /// </summary>
        /// <param name="type">The <see cref="FirearmType"/> to iterate over.</param>
        /// <param name="code">The <see cref="uint"/> value which represents the attachments code to check.</param>
        /// <param name="identifiers">The attachments present on the specified <see cref="FirearmType"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="FirearmType"/> is a weapon.</returns>
        public static bool TryGetAttachments(this FirearmType type, uint code, out IEnumerable<AttachmentIdentifier> identifiers)
        {
            identifiers = default;

            if (type is FirearmType.None)
                return false;

            identifiers = GetAttachmentIdentifiers(type, code);

            return true;
        }

        /// <summary>
        /// Gets the value resulting from the sum of all elements within a specific <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifiers">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to compute.</param>
        /// <returns>A <see cref="uint"/> value that represents the attachments code.</returns>
        public static uint GetAttachmentsCode(this IEnumerable<AttachmentIdentifier> identifiers) => identifiers.Aggregate<AttachmentIdentifier, uint>(0, (current, identifier) => current + identifier);

        /// <summary>
        /// Gets the base code of the specified <see cref="FirearmType"/>.
        /// </summary>
        /// <param name="type">The <see cref="FirearmType"/> to check.</param>
        /// <returns>The corresponding base code.</returns>
        public static uint GetBaseCode(this FirearmType type)
        {
            if (type is FirearmType.None)
                return 0;
            else if (Firearm.BaseCodesValue.TryGetValue(type, out uint baseCode))
                return baseCode;
            else
                throw new KeyNotFoundException($"Basecode for weapon {type} not found! Stored BaseCodesValue:\n{Firearm.BaseCodesValue.Keys.ToString(true)}\n{Firearm.BaseCodesValue.Values.ToString(true)}");
        }

        /// <summary>
        /// Gets the <see cref="ItemCategory"/> of the specified <see cref="ItemType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to check.</param>
        /// <returns><see cref="ItemCategory"/> of the specified <see cref="ItemType"/>.</returns>
        public static ItemCategory GetCategory(this ItemType type) => GetItemBase(type).Category;
    }
}
