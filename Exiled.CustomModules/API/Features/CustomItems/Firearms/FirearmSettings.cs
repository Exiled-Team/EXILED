// -----------------------------------------------------------------------
// <copyright file="FirearmSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Firearms
{
    using System;

    using CameraShaking;

    using Exiled.API.Extensions;
    using Exiled.CustomModules.API.Enums;

    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// A tool to easily setup firearms.
    /// </summary>
    public class FirearmSettings : ItemSettings
    {
        /// <inheritdoc/>
        public override ItemType ItemType
        {
            get => base.ItemType;
            set
            {
                if (!value.IsWeapon(false) && value != ItemType.None)
                    throw new ArgumentOutOfRangeException($"{nameof(Type)}", value, "Invalid weapon type.");

                base.ItemType = value;
            }
        }

        /// <summary>
        /// Gets or sets the firearm's ammo type.
        /// </summary>
        public ItemType AmmoType { get; set; }

        /// <summary>
        /// Gets or sets the firearm's attachments.
        /// </summary>
        public AttachmentName[] Attachments { get; set; } = { };

        /// <summary>
        /// Gets or sets the <see cref="Enums.FiringMode"/>.
        /// </summary>
        public FiringMode FiringMode { get; set; }

        /// <summary>
        /// Gets or sets the firearm's damage.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets or sets the size of the firearm's clip.
        /// </summary>
        public byte ClipSize { get; set; }

        /// <summary>
        /// Gets or sets the firearm's fire rate.
        /// <para/>
        /// Only decreasing is supported by non-automatic firearms.
        /// <br/>
        /// Automatic firearms are not supported.
        /// </summary>
        public byte FireRate { get; set; }

        /// <summary>
        /// Gets or sets the burst length.
        /// <para/>
        /// Only firearms with <see cref="FiringMode.Burst"/> will be affected.
        /// </summary>
        public byte BurstLength { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CameraShaking.RecoilSettings"/>.
        /// </summary>
        public RecoilSettings RecoilSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire is allowed with this firearm on FF-enabled servers.
        /// </summary>
        public bool AllowFriendlyFire { get; set; }
    }
}
