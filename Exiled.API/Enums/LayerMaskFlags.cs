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
        Default = 1 << 0,

        /// <summary>
        /// TransparentFX.
        /// </summary>
        TransparentFX = 1 << 1,

        /// <summary>
        /// IgnoreRaycast.
        /// </summary>
        IgnoreRaycast = 1 << 2,

        /// <summary>
        /// Water.
        /// </summary>
        Water = 1 << 4,

        /// <summary>
        /// UI.
        /// </summary>
        UI = 1 << 5,

        /// <summary>
        /// Player.
        /// </summary>
        Player = 1 << 8,

        /// <summary>
        /// InteractableNoPlayerCollision.
        /// </summary>
        InteractableNoPlayerCollision = 1 << 9,

        /// <summary>
        /// Viewmodel.
        /// </summary>
        Viewmodel = 1 << 10,

        /// <summary>
        /// RenderAfterFog.
        /// </summary>
        RenderAfterFog = 1 << 12,

        /// <summary>
        /// Hitbox.
        /// </summary>
        Hitbox = 1 << 13,

        /// <summary>
        /// Glass.
        /// </summary>
        Glass = 1 << 14,

        /// <summary>
        /// InvisibleCollider.
        /// </summary>
        InvisibleCollider = 1 << 16,

        /// <summary>
        /// Ragdoll.
        /// </summary>
        Ragdoll = 1 << 17,

        /// <summary>
        /// CCTV.
        /// </summary>
        CCTV = 1 << 18,

        /// <summary>
        /// Grenade.
        /// </summary>
        Grenade = 1 << 20,

        /// <summary>
        /// Phantom.
        /// </summary>
        Phantom = 1 << 21,

        /// <summary>
        /// OnlyWorldCollision.
        /// </summary>
        OnlyWorldCollision = 1 << 25,

        /// <summary>
        /// Door.
        /// </summary>
        Door = 1 << 27,

        /// <summary>
        /// Skybox.
        /// </summary>
        Skybox = 1 << 28,
    }
}
