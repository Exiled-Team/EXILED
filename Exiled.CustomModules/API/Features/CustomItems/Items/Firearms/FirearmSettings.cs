// -----------------------------------------------------------------------
// <copyright file="FirearmSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Firearms
{
    using System.ComponentModel;

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
        [Description("Indicates whether the custom reload logic should be used. It adds support for non-AmmoType items.")]
        public virtual bool OverrideReload { get; set; }

        /// <summary>
        /// Gets or sets the firearm's ammo type.
        /// <para/>
        /// This property cannot be used along with <see cref="CustomAmmoType"/>.
        /// <br/>
        /// <see cref="OverrideReload"/> is required to be set to <see langword="true"/> in case of non-<see cref="Exiled.API.Enums.AmmoType"/> items.
        /// </summary>
        [Description(
            "The firearm's ammo type. This property cannot be used along with CustomAmmoType." +
            "OverrideReload is required to be set to true in case of not AmmoType items.")]
        public virtual ItemType AmmoType { get; set; }

        /// <summary>
        /// Gets or sets the firearm's custom ammo type.
        /// <para/>
        /// This property cannot be used along with <see cref="AmmoType"/>.
        /// <br/>
        /// <see cref="OverrideReload"/> is required to be set to <see langword="true"/>.
        /// </summary>
        [Description("The firearm's ammo type. This property cannot be used along with AmmoType. OverrideReload is required to be set to true.")]
        public virtual uint CustomAmmoType { get; set; }

        /// <summary>
        /// Gets or sets the firearm's attachments.
        /// </summary>
        [Description("The firearm's attachments.")]
        public virtual AttachmentName[] Attachments { get; set; } = { };

        /// <summary>
        /// Gets or sets the <see cref="Enums.FiringMode"/>.
        /// </summary>
        [Description("The firearm's firing mode.")]
        public virtual FiringMode FiringMode { get; set; }

        /// <summary>
        /// Gets or sets the firearm's damage.
        /// </summary>
        [Description("The firearm's damage.")]
        public virtual float Damage { get; set; }

        /// <summary>
        /// Gets or sets the firearm's max ammo.
        /// </summary>
        [Description("The firearm's max ammo.")]
        public virtual byte MaxAmmo { get; set; }

        /// <summary>
        /// Gets or sets the size of the firearm's clip.
        /// </summary>
        [Description("The firearm's clip size.")]
        public virtual byte ClipSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the firearm's chamber.
        /// <para/>
        /// <see cref="OverrideReload"/> is required to be set to <see langword="true"/>.
        /// </summary>
        [Description("The firearm's chamber size. OverrideReload is required to be set to true.")]
        public virtual int ChamberSize { get; set; }

        /// <summary>
        /// Gets or sets the firearm's fire rate.
        /// <para/>
        /// Only decreasing is supported by non-automatic firearms.
        /// <br/>
        /// Automatic firearms are not supported.
        /// </summary>
        [Description("The firearm's fire rate. Only decreasing is supported by non-automatic firearms; automatic firearms are not supported.")]
        public virtual byte FireRate { get; set; }

        /// <summary>
        /// Gets or sets the burst length.
        /// <para/>
        /// Only firearms with <see cref="FiringMode.Burst"/> will be affected.
        /// </summary>
        [Description("The firearm's burst length. Only firearms with Burst fire mode will be affected.")]
        public virtual byte BurstLength { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CameraShaking.RecoilSettings"/>.
        /// </summary>
        [Description("The firearm's recoil.")]
        public virtual RecoilSettings RecoilSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire is allowed with this firearm on FF-enabled servers.
        /// </summary>
        [Description("Indicates whether friendly fire is allowed with this firearm on FF-enabled servers.")]
        public virtual bool AllowFriendlyFire { get; set; }
    }
}
