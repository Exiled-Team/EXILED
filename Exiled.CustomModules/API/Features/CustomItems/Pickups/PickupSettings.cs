// -----------------------------------------------------------------------
// <copyright file="PickupSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Pickups
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Spawn;
    using UnityEngine;

    /// <summary>
    /// A tool to easily setup pickups.
    /// </summary>
    public class PickupSettings : Settings
    {
        /// <summary>
        /// Gets the default <see cref="PickupSettings"/> values.
        /// <para>It refers to the base-game pickup behavior.</para>
        /// </summary>
        public static PickupSettings Default { get; } = new();

        /// <summary>
        /// Gets or sets the custom pickup's <see cref="global::ItemType"/>.
        /// </summary>
        public override ItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Features.Spawn.SpawnProperties"/>.
        /// </summary>
        public override SpawnProperties SpawnProperties { get; set; } = new();

        /// <summary>
        /// Gets or sets the weight of the pickup.
        /// </summary>
        public override float Weight { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the scale of the pickup.
        /// </summary>
        public override Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> to be displayed when the pickup has been picked up.
        /// </summary>
        public override TextDisplay PickedUpText { get; set; }
    }
}
