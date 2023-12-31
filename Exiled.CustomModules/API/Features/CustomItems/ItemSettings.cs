// -----------------------------------------------------------------------
// <copyright file="ItemSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;

    using UnityEngine;

    /// <summary>
    /// A tool to easily setup Items.
    /// </summary>
    public class ItemSettings : TypeCastObject<ItemSettings>, IAdditiveProperty
    {
        /// <summary>
        /// Gets the default <see cref="ItemSettings"/> values.
        /// <para>It refers to the base-game item behavior.</para>
        /// </summary>
        public static ItemSettings Default { get; } = new();

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        public virtual float Weight { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the scale of the item.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Hint"/> to be displayed when the item has been picked up.
        /// </summary>
        public virtual Hint PickedUpHint { get; set; }
    }
}
