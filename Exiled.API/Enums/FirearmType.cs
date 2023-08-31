// -----------------------------------------------------------------------
// <copyright file="FirearmType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Represents a firearm.
    /// </summary>
    /// <seealso cref="Extensions.ItemExtensions.GetAttachmentIdentifiers(FirearmType, uint)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetBaseCode(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetFirearmType(ItemType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetItemType(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetMaxAmmo(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetWeaponAmmoType(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.TryGetAttachments(FirearmType, uint, out System.Collections.Generic.IEnumerable{Structs.AttachmentIdentifier})"/>
    /// <seealso cref="Features.Items.Firearm.FirearmType"/>
    public enum FirearmType
    {
        /// <summary>
        /// Not a firearm.
        /// </summary>
        None,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCOM15"/>.
        /// </summary>
        Com15,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCOM18"/>.
        /// </summary>
        Com18,

        /// <summary>
        /// Represents the <see cref="ItemType.GunE11SR"/>.
        /// </summary>
        E11SR,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCrossvec"/>.
        /// </summary>
        Crossvec,

        /// <summary>
        /// Represents the <see cref="ItemType.GunFSP9"/>.
        /// </summary>
        FSP9,

        /// <summary>
        /// Represents the <see cref="ItemType.GunLogicer"/>.
        /// </summary>
        Logicer,

        /// <summary>
        /// Represents the <see cref="ItemType.GunRevolver"/>.
        /// </summary>
        Revolver,

        /// <summary>
        /// Represents the <see cref="ItemType.GunAK"/>.
        /// </summary>
        AK,

        /// <summary>
        /// Represents the <see cref="ItemType.GunShotgun"/>.
        /// </summary>
        Shotgun,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCom45"/>.
        /// </summary>
        Com45,

        /// <summary>
        /// Represents the <see cref="ItemType.ParticleDisruptor"/>.
        /// </summary>
        ParticleDisruptor,

        /// <summary>
        /// Represents the <see cref="ItemType.GunFRMG0"/>.
        /// </summary>
        FRMG0,
    }
}
