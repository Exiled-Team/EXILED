// -----------------------------------------------------------------------
// <copyright file="CandySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using Exiled.API.Features;
    using InventorySystem.Items.Usables.Scp330;
    using UnityEngine;

    /// <summary>
    /// A tool to easily setup candies.
    /// </summary>
    public class CandySettings : Settings
    {
        /// <summary>
        /// Gets or sets a <see cref="CandyKindID"/> of a custom candy.
        /// </summary>
        public virtual CandyKindID CandyType { get; set; }

        /// <summary>
        /// Gets or sets chance that player would get a custom candy.
        /// </summary>
        public override float Weight
        {
            get => base.Weight;
            set => base.Weight = Mathf.Clamp(value, 0, 100);
        }

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> that will be displayed when player ate custom candy..
        /// </summary>
        public virtual TextDisplay EatenCustomCandyMessage { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="TextDisplay"/> that will be displayed when player has received custom candy.
        /// </summary>
        public virtual TextDisplay ReceivedCustomCandyMessage { get; set; }

        /// <inheritdoc/>
        public override TextDisplay SelectedText { get; set; }
    }
}