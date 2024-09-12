// -----------------------------------------------------------------------
// <copyright file="GrenadeSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Explosives
{
    using System.ComponentModel;

    /// <summary>
    /// A tool to easily setup grenades.
    /// </summary>
    public class GrenadeSettings : Settings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the grenade should explode immediately when contacting any surface.
        /// </summary>
        [Description("Indicates whether the grenade should explode immediately when contacting any surface.")]
        public virtual bool ExplodeOnCollision { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long the grenade's fuse time should be.
        /// </summary>
        [Description("Indicates how long the grenade's fuse time should be.")]
        public virtual float FuseTime { get; set; }
    }
}
