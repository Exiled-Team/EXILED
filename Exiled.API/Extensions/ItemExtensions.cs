// -----------------------------------------------------------------------
// <copyright file="ItemExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    using UnityEngine;

    using Object = UnityEngine.Object;

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
        public static bool IsAmmo(this ItemType item) => item == ItemType.Ammo9X19 || item == ItemType.Ammo12Gauge || item == ItemType.Ammo44Cal || item == ItemType.Ammo556X45 || item == ItemType.Ammo762X39;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a weapon.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <param name="checkMicro">Indicates whether the MicroHID item should be taken into account or not.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a weapon or not.</returns>
        public static bool IsWeapon(this ItemType type, bool checkMicro = true) => type == ItemType.GunCrossvec ||
            type == ItemType.GunLogicer || type == ItemType.GunRevolver || type == ItemType.GunShotgun ||
            type == ItemType.GunAk || type == ItemType.GunCom15 || type == ItemType.GunCom18 ||
            type == ItemType.GunE11Sr || type == ItemType.GunFsp9 || (checkMicro && type == ItemType.MicroHid);

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is an SCP.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is an SCP or not.</returns>
        public static bool IsScp(this ItemType type) => type == ItemType.Scp018 || type == ItemType.Scp500 || type == ItemType.Scp268 || type == ItemType.Scp207;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a throwable item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a throwable item or not.</returns>
        public static bool IsThrowable(this ItemType type) => type == ItemType.Scp018 || type == ItemType.GrenadeHe || type == ItemType.GrenadeFlash;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a medical item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a medical item or not.</returns>
        public static bool IsMedical(this ItemType type) => type == ItemType.Painkillers || type == ItemType.Medkit || type == ItemType.Scp500 || type == ItemType.Adrenaline;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a utility item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is an utilty item or not.</returns>
        public static bool IsUtility(this ItemType type) => /*type == ItemType.Disarmer ||*/ type == ItemType.Flashlight || type == ItemType.Radio;

        /// <summary>
        /// Check if a <see cref="ItemType"/> is an armor item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is an armor or not.</returns>
        public static bool IsArmor(this ItemType type) => type == ItemType.ArmorCombat || type == ItemType.ArmorHeavy ||
                                                          type == ItemType.ArmorLight;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a keycard.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a keycard or not.</returns>
        public static bool IsKeycard(this ItemType type) =>
            type == ItemType.KeycardChaosInsurgency || type == ItemType.KeycardContainmentEngineer || type == ItemType.KeycardFacilityManager ||
            type == ItemType.KeycardGuard || type == ItemType.KeycardJanitor || type == ItemType.KeycardNtfCommander ||
            type == ItemType.KeycardNtfLieutenant || type == ItemType.KeycardO5 || type == ItemType.KeycardScientist ||
            type == ItemType.KeycardResearchCoordinator || type == ItemType.KeycardNtfOfficer || type == ItemType.KeycardZoneManager;

        /// <summary>
        /// Gets the default ammo of a weapon.
        /// </summary>
        /// <param name="item">The <see cref="ItemType">item</see> that you want to get durability of.</param>
        /// <returns>Returns the item durability.</returns>
        public static byte GetMaxAmmo(this ItemType item)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue((global::ItemType)item, out var itemBase) || !(itemBase is InventorySystem.Items.Firearms.Firearm firearm))
                return 0;

            return firearm.AmmoManagerModule.MaxAmmo;
        }

        /// <summary>
        /// Converts a valid ammo <see cref="ItemType"/> into an <see cref="AmmoType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to convert.</param>
        /// <returns>The ammo type of the given item type.</returns>
        public static AmmoType GetAmmoType(this ItemType type)
        {
            switch (type)
            {
                case ItemType.Ammo9X19:
                    return AmmoType.Nato9;
                case ItemType.Ammo556X45:
                    return AmmoType.Nato556;
                case ItemType.Ammo762X39:
                    return AmmoType.Nato762;
                case ItemType.Ammo12Gauge:
                    return AmmoType.Ammo12Gauge;
                case ItemType.Ammo44Cal:
                    return AmmoType.Ammo44Cal;
                default:
                    return AmmoType.None;
            }
        }

        /// <summary>
        /// Converts an <see cref="AmmoType"/> into it's corresponding <see cref="ItemType"/>.
        /// </summary>
        /// <param name="type">The <see cref="AmmoType"/> to convert.</param>
        /// <returns>The Item type of the specified ammo.</returns>
        public static ItemType GetItemType(this AmmoType type)
        {
            switch (type)
            {
                case AmmoType.Nato556:
                    return ItemType.Ammo556X45;
                case AmmoType.Nato762:
                    return ItemType.Ammo762X39;
                case AmmoType.Nato9:
                    return ItemType.Ammo9X19;
                case AmmoType.Ammo12Gauge:
                    return ItemType.Ammo12Gauge;
                case AmmoType.Ammo44Cal:
                    return ItemType.Ammo44Cal;
                default:
                    return ItemType.None;
            }
        }
    }
}
