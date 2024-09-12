// -----------------------------------------------------------------------
// <copyright file="AmmoSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Pickups.Ammos
{
    using System.ComponentModel;

    /// <summary>
    /// A tool to easily setup ammos.
    /// </summary>
    public class AmmoSettings : SettingsBase
    {
        /// <summary>
        /// Gets or sets the sizes of the ammo box.
        /// </summary>
        [Description("The sizes of the ammo box.")]
        public virtual ushort[] BoxSizes { get; set; } = { };

        /// <summary>
        /// Gets or sets the maximum allowed amount of ammo.
        /// </summary>
        [Description("The maximum allowed amount of ammo.")]
        public virtual ushort MaxUnits { get; set; }
    }
}
