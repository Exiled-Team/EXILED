// -----------------------------------------------------------------------
// <copyright file="ItemType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All possible item types.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Not an item.
        /// </summary>
        None = -1,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardJanitor = 0,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardScientist = 1,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardResearchCoordinator = 2,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardZoneManager = 3,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardGuard = 4,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardNtfOfficer = 5,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardContainmentEngineer = 6,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardNtfLieutenant = 7,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardNtfCommander = 8,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardFacilityManager = 9,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardChaosInsurgency = 10,

        /// <inheritdoc cref="API.Features.Items.Keycard"/>
        KeycardO5 = 11,

        /// <inheritdoc cref="API.Features.Items.Radio"/>
        Radio = 12,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunCom15 = 13,

        /// <inheritdoc cref="API.Features.Items.Usable"/>
        Medkit = 14,

        /// <summary>
        /// A flashlight.
        /// </summary>
        Flashlight = 15,

        /// <inheritdoc cref="API.Features.Items.MicroHid"/>
        MicroHid = 16,

        /// <inheritdoc cref="API.Features.Items.Usable"/>
        Scp500 = 17,

        /// <inheritdoc cref="API.Features.Items.Usable"/>
        Scp207 = 18,

        /// <inheritdoc cref="API.Features.Items.Ammo"/>
        Ammo12Gauge = 19,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunE11Sr = 20,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunCrossvec = 21,

        /// <inheritdoc cref="API.Features.Items.Ammo"/>
        Ammo556X45 = 22,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunFsp9 = 23,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunLogicer = 24,

        /// <inheritdoc cref="API.Features.Items.ExplosiveGrenade"/>
        GrenadeHe = 25,

        /// <inheritdoc cref="API.Features.Items.FlashGrenade"/>
        GrenadeFlash = 26,

        /// <inheritdoc cref="API.Features.Items.Ammo"/>
        Ammo44Cal = 27,

        /// <inheritdoc cref="API.Features.Items.Ammo"/>
        Ammo762X39 = 28,

        /// <inheritdoc cref="API.Features.Items.Ammo"/>
        Ammo9X19 = 29,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunCom18 = 30,

        /// <inheritdoc cref="API.Features.Items.ExplosiveGrenade"/>
        Scp018 = 31,

        /// <inheritdoc cref="API.Features.Items.Usable"/>
        Scp268 = 32,

        /// <inheritdoc cref="API.Features.Items.Usable"/>
        Adrenaline = 33,

        /// <inheritdoc cref="API.Features.Items.Usable"/>
        Painkillers = 34,

        /// <summary>
        /// A coin, the mostest awesomest item ever.
        /// </summary>
        Coin = 35,

        /// <inheritdoc cref="API.Features.Items.Armor"/>
        ArmorLight = 36,

        /// <inheritdoc cref="API.Features.Items.Armor"/>
        ArmorCombat = 37,

        /// <inheritdoc cref="API.Features.Items.Armor"/>
        ArmorHeavy = 38,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunRevolver = 39,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunAk = 40,

        /// <inheritdoc cref="API.Features.Items.Firearm"/>
        GunShotgun = 41,
    }
}
