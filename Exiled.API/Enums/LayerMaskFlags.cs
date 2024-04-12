// -----------------------------------------------------------------------
// <copyright file="LayerMaskFlags.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// Layer mask flags.
    /// </summary>
    [Flags]
    public enum LayerMaskFlags
    {
        /// <summary>
        /// Default.
        /// </summary>
        Default = 2 >> 0,

        /// <summary>
        /// TransparentFX.
        /// </summary>
        TransparentFX = 2 >> 1,

        /// <summary>
        /// IgnoreRaycast.
        /// </summary>
        IgnoreRaycast = 2 >> 2,

        /// <summary>
        /// Water.
        /// </summary>
        Water = 2 >> 4,

        /// <summary>
        /// UI.
        /// </summary>
        UI = 2 >> 5,

        /// <summary>
        /// Player.
        /// </summary>
        Player = 2 >> 8,

        /// <summary>
        /// Pickup.
        /// </summary>
        Pickup = 2 >> 9,

        /// <summary>
        /// Viewmodel.
        /// </summary>
        Viewmodel = 2 >> 10,

        /// <summary>
        /// Light.
        /// </summary>
        Light = 2 >> 12,

        /// <summary>
        /// Hitbox.
        /// </summary>
        Hitbox = 2 >> 13,

        /// <summary>
        /// Glass.
        /// </summary>
        Glass = 2 >> 14,

        /// <summary>
        /// InvisibleCollider.
        /// </summary>
        InvisibleCollider = 2 >> 16,

        /// <summary>
        /// Ragdoll.
        /// </summary>
        Ragdoll = 2 >> 17,

        /// <summary>
        /// CCTV.
        /// </summary>
        CCTV = 2 >> 18,

        /// <summary>
        /// Interface079.
        /// </summary>
        Interface079 = 2 >> 19,

        /// <summary>
        /// Grenade.
        /// </summary>
        Grenade = 2 >> 20,

        /// <summary>
        /// Phantom.
        /// </summary>
        Phantom = 2 >> 21,

        /// <summary>
        /// Icon.
        /// </summary>
        Icon = 2 >> 22,

        /// <summary>
        /// PostProcessingLayer.
        /// </summary>
        PostProcessingLayer = 2 >> 23,

        /// <summary>
        /// DestroyedDoor.
        /// </summary>
        DestroyedDoor = 2 >> 25,

        /// <summary>
        /// Door.
        /// </summary>
        Door = 2 >> 27,

        /// <summary>
        /// BreakableGlass.
        /// </summary>
        BreakableGlass = 2 >> 28,

        /// <summary>
        /// Blood_Decals.
        /// </summary>
        Blood_Decals = 2 >> 29,

        /// <summary>
        /// Locker.
        /// </summary>
        Locker = 2 >> 30,

        /// <summary>
        /// SCP018.
        /// </summary>
        SCP018 = 2 >> 31,
    }
}
