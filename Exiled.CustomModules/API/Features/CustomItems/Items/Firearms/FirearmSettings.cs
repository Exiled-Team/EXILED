// -----------------------------------------------------------------------
// <copyright file="FirearmSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Firearms
{
    using CameraShaking;
    using Exiled.CustomModules.API.Enums;
    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// A tool to easily setup firearms.
    /// </summary>
    public class FirearmSettings : SettingsBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the custom reload logic should be used.
        /// <para/>
        /// It adds support for non-<see cref="Exiled.API.Enums.AmmoType"/> items.
        /// </summary>
        public virtual bool OverrideReload { get; set; }

        /// <summary>
        /// Gets or sets the firearm's ammo type.
        /// <para/>
        /// This property cannot be used along with <see cref="CustomAmmoType"/>.
        /// <br/>
        /// <see cref="OverrideReload"/> is required to be set to <see langword="true"/> in case of non-<see cref="Exiled.API.Enums.AmmoType"/> items.
        /// </summary>
        public virtual ItemType AmmoType { get; set; }

        /// <summary>
        /// Gets or sets the firearm's custom ammo type.
        /// <para/>
        /// This property cannot be used along with <see cref="AmmoType"/>.
        /// <br/>
        /// <see cref="OverrideReload"/> is required to be set to <see langword="true"/>.
        /// </summary>
        public virtual uint CustomAmmoType { get; set; }

        /// <summary>
        /// Gets or sets the firearm's attachments.
        /// </summary>
        public virtual AttachmentName[] Attachments { get; set; } = { };

        /// <summary>
        /// Gets or sets the <see cref="Enums.FiringMode"/>.
        /// </summary>
        public virtual FiringMode FiringMode { get; set; }

        /// <summary>
        /// Gets or sets the firearm's damage.
        /// </summary>
        public virtual float Damage { get; set; }

        /// <summary>
        /// Gets or sets the firearm's max ammo.
        /// </summary>
        public virtual byte MaxAmmo { get; set; }

        /// <summary>
        /// Gets or sets the size of the firearm's clip.
        /// </summary>
        public virtual byte ClipSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the firearm's chamber.
        /// <para/>
        /// <see cref="OverrideReload"/> is required to be set to <see langword="true"/>.
        /// </summary>
        public virtual int ChamberSize { get; set; }

        /// <summary>
        /// Gets or sets the firearm's fire rate.
        /// <para/>
        /// Only decreasing is supported by non-automatic firearms.
        /// <br/>
        /// Automatic firearms are not supported.
        /// </summary>
        public virtual byte FireRate { get; set; }

        /// <summary>
        /// Gets or sets the burst length.
        /// <para/>
        /// Only firearms with <see cref="FiringMode.Burst"/> will be affected.
        /// </summary>
        public virtual byte BurstLength { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CameraShaking.RecoilSettings"/>.
        /// </summary>
        public virtual RecoilSettings RecoilSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire is allowed with this firearm on FF-enabled servers.
        /// </summary>
        public virtual bool AllowFriendlyFire { get; set; }
    }
}
