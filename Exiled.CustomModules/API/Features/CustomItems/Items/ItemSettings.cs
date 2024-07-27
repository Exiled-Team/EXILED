// -----------------------------------------------------------------------
// <copyright file="ItemSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items
{
    using Exiled.API.Features;
    using Exiled.API.Features.Spawn;

    using UnityEngine;

    /// <summary>
    /// A tool to easily setup items.
    /// </summary>
    public class ItemSettings : Settings
    {
        /// <summary>
        /// Gets the default <see cref="ItemSettings"/> values.
        /// <para>It refers to the base-game item behavior.</para>
        /// </summary>
        public static ItemSettings Default { get; } = new();

        /// <summary>
        /// Gets or sets the custom item's <see cref="global::ItemType"/>.
        /// </summary>
        public override ItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Features.Spawn.SpawnProperties"/>.
        /// </summary>
        public override SpawnProperties SpawnProperties { get; set; } = new();

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        public override float Weight { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the scale of the item.
        /// </summary>
        public override Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets a value indicating whether or not this item causes things to happen that may be considered hacks, and thus be shown to global moderators as being present in a player's inventory when they gban them.
        /// </summary>
        public virtual bool ShouldMessageOnGban { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> to be displayed when the item has been picked up.
        /// </summary>
        public override TextDisplay PickedUpText { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> to be displayed when the item has been selected.
        /// </summary>
        public virtual TextDisplay SelectedText { get; set; }
    }
}
