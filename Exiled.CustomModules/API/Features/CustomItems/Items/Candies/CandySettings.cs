// -----------------------------------------------------------------------
// <copyright file="CandySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using System.ComponentModel;

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
        [Description("The CandyKindID of a custom candy.")]
        public virtual CandyKindID CandyType { get; set; }

        /// <summary>
        /// Gets or sets the chance of getting a custom candy.
        /// </summary>
        [Description("The chance of getting a custom candy.")]
        public override float Weight
        {
            get => base.Weight;
            set => base.Weight = Mathf.Clamp(value, 0, 100);
        }

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> to be displayed when a player ate the custom candy.
        /// </summary>
        [Description("The TextDisplay to be displayed when a player ate the custom candy.")]
        public virtual TextDisplay EatenCustomCandyText { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="TextDisplay"/> that will be displayed when player has received custom candy.
        /// </summary>
        [Description("The TextDisplay to be displayed when a player has received a custom candy.")]
        public virtual TextDisplay ReceivedCustomCandyText { get; set; }
    }
}