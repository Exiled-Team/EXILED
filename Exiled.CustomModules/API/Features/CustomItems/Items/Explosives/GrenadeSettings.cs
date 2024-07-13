// -----------------------------------------------------------------------
// <copyright file="GrenadeSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Explosives
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.CustomModules.API.Features.CustomItems.Items;

    /// <summary>
    /// A tool to easily setup grenades.
    /// </summary>
    public class GrenadeSettings : ItemSettings
    {
        /// <inheritdoc/>
        public override ItemType ItemType
        {
            get => base.ItemType;
            set
            {
                if (!value.IsThrowable() && value != ItemType.None)
                    throw new ArgumentOutOfRangeException($"{nameof(Type)}", value, "Invalid grenade type.");

                base.ItemType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grenade should explode immediately when contacting any surface.
        /// </summary>
        public virtual bool ExplodeOnCollision { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long the grenade's fuse time should be.
        /// </summary>
        public virtual float FuseTime { get; set; }
    }
}
