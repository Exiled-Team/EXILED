// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="ItemType"/>.
    /// </summary>
    public static class Item
    {
        /// <summary>
        /// Spawns an <see cref="ItemType"/> in a desired <see cref="Vector3"/> position.
        /// </summary>
        /// <param name="itemType">The type of the item to be spawned.</param>
        /// <param name="durability">The durability (or ammo, depends on the weapon) of the item.</param>
        /// <param name="position">Where the item will be spawned.</param>
        /// <param name="rotation">The rotation. We recommend you to use <see cref="Quaternion.Euler(float, float, float)"/>.</param>
        /// <param name="sight">The sight the weapon will have (0 is nothing, 1 is the first sight available in the weapon manager, and so on).</param>
        /// <param name="barrel">The barrel of the weapon (0 is no custom barrel, 1 is the first barrel available, and so on).</param>
        /// <param name="other">Other attachments like flashlight, laser or ammo counter.</param>
        /// <returns>Returns the spawned <see cref="Pickup"/>.</returns>
        public static Pickup Spawn(this ItemType itemType, float durability, Vector3 position, Quaternion rotation = default, int sight = 0, int barrel = 0, int other = 0) => Server.Host.Inventory.SetPickup(itemType, durability, position, rotation, sight, barrel, other);

        /// <summary>
        /// Set the ammo of an <see cref="Inventory.SyncItemInfo">item</see>.
        /// </summary>
        /// <param name="list">The list of items.</param>
        /// <param name="weapon">The weapon to be changed.</param>
        /// <param name="amount">The ammo amount.</param>
        public static void SetWeaponAmmo(this Inventory.SyncListItemInfo list, Inventory.SyncItemInfo weapon, int amount) => list.ModifyDuration(list.IndexOf(weapon), amount);

        /// <summary>
        /// Set the ammo value of an <see cref="Inventory.SyncItemInfo"/>.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="weapon">The weapon to be changed.</param>
        /// <param name="amount">The ammo amount.</param>
        public static void SetWeaponAmmo(this Player player, Inventory.SyncItemInfo weapon, int amount) => player.Inventory.items.ModifyDuration(player.Inventory.items.IndexOf(weapon), amount);

        /// <summary>
        /// Get the ammo of an <see cref="Inventory.SyncItemInfo"/>.
        /// </summary>
        /// <param name="weapon">The weapon to be get.</param>
        /// <returns>Returns the weapon left ammo.</returns>
        public static float GetWeaponAmmo(this Inventory.SyncItemInfo weapon) => weapon.durability;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is an ammo.
        /// </summary>
        /// <param name="item">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is an ammo or not.</returns>
        public static bool IsAmmo(this ItemType item) => item == ItemType.Ammo556 || item == ItemType.Ammo9mm || item == ItemType.Ammo762;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a weapon.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <param name="checkMicro">Indicates whether the MicroHID item should be taken into account or not.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a weapon or not.</returns>
        public static bool IsWeapon(this ItemType type, bool checkMicro = true) =>
            type == ItemType.GunCOM15 || type == ItemType.GunE11SR || type == ItemType.GunLogicer ||
            type == ItemType.GunMP7 || type == ItemType.GunProject90 || type == ItemType.GunUSP || (checkMicro && type == ItemType.MicroHID);

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is an SCP.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is an SCP or not.</returns>
        public static bool IsSCP(this ItemType type) => type == ItemType.SCP018 || type == ItemType.SCP500 || type == ItemType.SCP268 || type == ItemType.SCP207;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a throwable item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a throwable item or not.</returns>
        public static bool IsThrowable(this ItemType type) => type == ItemType.SCP018 || type == ItemType.GrenadeFrag || type == ItemType.GrenadeFlash;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a medical item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a medical item or not.</returns>
        public static bool IsMedical(this ItemType type) => type == ItemType.Painkillers || type == ItemType.Medkit || type == ItemType.SCP500 || type == ItemType.Adrenaline;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a utility item.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is an utilty item or not.</returns>
        public static bool IsUtility(this ItemType type) => type == ItemType.Disarmer || type == ItemType.Flashlight || type == ItemType.Radio || type == ItemType.WeaponManagerTablet;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a keycard.
        /// </summary>
        /// <param name="type">The item to be checked.</param>
        /// <returns>Returns whether the <see cref="ItemType"/> is a keycard or not.</returns>
        public static bool IsKeycard(this ItemType type) =>
            type == ItemType.KeycardChaosInsurgency || type == ItemType.KeycardContainmentEngineer || type == ItemType.KeycardFacilityManager ||
            type == ItemType.KeycardGuard || type == ItemType.KeycardJanitor || type == ItemType.KeycardNTFCommander ||
            type == ItemType.KeycardNTFLieutenant || type == ItemType.KeycardO5 || type == ItemType.KeycardScientist ||
            type == ItemType.KeycardScientistMajor || type == ItemType.KeycardSeniorGuard || type == ItemType.KeycardZoneManager;

        /// <summary>
        /// Gets sight modification of the weapon.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="weapon">The weapon with attachment.</param>
        /// <returns>Returns <see cref="SightType"/>.</returns>
        public static SightType GetSight(this Player player, Inventory.SyncItemInfo weapon)
        {
            WeaponManager wmanager = player.ReferenceHub.weaponManager;
            if (weapon.id.IsWeapon())
            {
                WeaponManager.Weapon wep = wmanager.weapons.Where(wp => wp.inventoryID == weapon.id).FirstOrDefault();
                if (wep != null)
                {
                    switch (wep.mod_sights[weapon.modSight].name)
                    {
                        case "Collimator":
                            return SightType.Collimator;
                        case "Holo Sight":
                            return SightType.Holo;
                        case "Red Dot":
                            return SightType.RedDot;
                        case "Blue Dot Sight":
                            return SightType.BlueDot;
                        case "Night Vision Sight":
                            return SightType.NightVision;
                        case "Sniper Scope":
                            return SightType.Sniper;
                    }
                }
            }

            return SightType.None;
        }

        /// <summary>
        /// Sets sight modification of the weapon.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="weapon">The weapon with attachment.</param>
        /// <param name="type">Type of the sight.</param>
        public static void SetSight(this Player player, Inventory.SyncItemInfo weapon, SightType type)
        {
            WeaponManager wmanager = player.ReferenceHub.weaponManager;
            if (weapon.id.IsWeapon())
            {
                WeaponManager.Weapon wep = wmanager.weapons.Where(wp => wp.inventoryID == weapon.id).FirstOrDefault();
                if (wep != null)
                {
                    string name = "None";
                    switch (type)
                    {
                        case SightType.Collimator:
                            name = "Collimator";
                            break;
                        case SightType.Holo:
                            name = "Holo Sight";
                            break;
                        case SightType.BlueDot:
                            name = "Blue Dot Sight";
                            break;
                        case SightType.RedDot:
                            name = "Red Dot";
                            break;
                        case SightType.NightVision:
                            name = "Night Vision Sight";
                            break;
                        case SightType.Sniper:
                            name = "Sniper Scope";
                            break;
                    }

                    int weaponMod = wep.mod_sights.Select((s, i) => new { s, i }).Where(e => e.s.name == name).Select(e => e.i).FirstOrDefault();
                    int weaponId = player.Inventory.items.FindIndex(s => s == weapon);
                    weapon.modSight = weaponMod;
                    player.Inventory.items[weaponId] = weapon;
                }
            }
        }

        /// <summary>
        /// Gets barrel modification of the weapon.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="weapon">The weapon with attachment.</param>
        /// <returns>Returns <see cref="BarrelType"/>.</returns>
        public static BarrelType GetBarrel(this Player player, Inventory.SyncItemInfo weapon)
        {
            WeaponManager wmanager = player.ReferenceHub.weaponManager;
            if (weapon.id.IsWeapon())
            {
                WeaponManager.Weapon wep = wmanager.weapons.Where(wp => wp.inventoryID == weapon.id).FirstOrDefault();
                if (wep != null)
                {
                    switch (wep.mod_barrels[weapon.modBarrel].name)
                    {
                        case "Suppressor":
                            return BarrelType.Suppressor;
                        case "Silencer":
                            return BarrelType.Silencer;
                        case "Muzzle Brake":
                            return BarrelType.MuzzleBrake;
                        case "Heavy Barrel":
                            return BarrelType.HeavyBarrel;
                        case "Muzzle Booster":
                            return BarrelType.MuzzleBooster;
                    }
                }
            }

            return BarrelType.None;
        }

        /// <summary>
        /// Sets barrel modification of the weapon.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="weapon">The weapon with attachment.</param>
        /// <param name="type">Type of the barrel.</param>
        public static void SetBarrel(this Player player, Inventory.SyncItemInfo weapon, BarrelType type)
        {
            WeaponManager wmanager = player.ReferenceHub.weaponManager;
            if (weapon.id.IsWeapon())
            {
                WeaponManager.Weapon wep = wmanager.weapons.Where(wp => wp.inventoryID == weapon.id).FirstOrDefault();
                if (wep != null)
                {
                    string name = "None";
                    switch (type)
                    {
                        case BarrelType.HeavyBarrel:
                            name = "Heavy Barrel";
                            break;
                        case BarrelType.MuzzleBooster:
                            name = "Muzzle Booster";
                            break;
                        case BarrelType.MuzzleBrake:
                            name = "Muzzle Brake";
                            break;
                        case BarrelType.Silencer:
                            name = "Silencer";
                            break;
                        case BarrelType.Suppressor:
                            name = "Suppressor";
                            break;
                    }

                    int weaponMod = wep.mod_barrels.Select((s, i) => new { s, i }).Where(e => e.s.name == name).Select(e => e.i).FirstOrDefault();
                    int weaponId = player.Inventory.items.FindIndex(s => s == weapon);
                    weapon.modBarrel = weaponMod;
                    player.Inventory.items[weaponId] = weapon;
                }
            }
        }

        /// <summary>
        /// Gets other modification of the weapon.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="weapon">The weapon with attachment.</param>
        /// <returns>Returns <see cref="OtherType"/>.</returns>
        public static OtherType GetOther(this Player player, Inventory.SyncItemInfo weapon)
        {
            WeaponManager wmanager = player.ReferenceHub.weaponManager;
            if (weapon.id.IsWeapon())
            {
                WeaponManager.Weapon wep = wmanager.weapons.Where(wp => wp.inventoryID == weapon.id).FirstOrDefault();
                if (wep != null)
                {
                    switch (wep.mod_others[weapon.modOther].name)
                    {
                        case "Flashlight":
                            return OtherType.Flashlight;
                        case "Ammo Counter":
                            return OtherType.AmmoCounter;
                        case "Gyroscopic Stabilizer":
                            return OtherType.GyroStabilizer;
                        case "Laser":
                            return OtherType.Laser;
                    }
                }
            }

            return OtherType.None;
        }

        /// <summary>
        /// Sets other modification of the weapon.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="weapon">The weapon with attachment.</param>
        /// <param name="type">Type of the other.</param>
        public static void SetOther(this Player player, Inventory.SyncItemInfo weapon, OtherType type)
        {
            WeaponManager wmanager = player.ReferenceHub.weaponManager;
            if (weapon.id.IsWeapon())
            {
                WeaponManager.Weapon wep = wmanager.weapons.Where(wp => wp.inventoryID == weapon.id).FirstOrDefault();
                if (wep != null)
                {
                    string name = "None";
                    switch (type)
                    {
                        case OtherType.AmmoCounter:
                            name = "Ammo Counter";
                            break;
                        case OtherType.Flashlight:
                            name = "Flashlight";
                            break;
                        case OtherType.GyroStabilizer:
                            name = "Gyroscopic Stabilizer";
                            break;
                        case OtherType.Laser:
                            name = "Laser";
                            break;
                    }

                    int weaponMod = wep.mod_others.Select((s, i) => new { s, i }).Where(e => e.s.name == name).Select(e => e.i).FirstOrDefault();
                    int weaponId = player.Inventory.items.FindIndex(s => s == weapon);
                    weapon.modOther = weaponMod;
                    player.Inventory.items[weaponId] = weapon;
                }
            }
        }
    }
}
