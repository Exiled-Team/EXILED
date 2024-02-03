// -----------------------------------------------------------------------
// <copyright file="CandySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using InventorySystem.Items.Usables.Scp330;
    using UnityEngine;

    /// <summary>
    /// A tool to easily setup candies.
    /// </summary>
    public class CandySettings : ItemSettings
    {
        /// <inheritdoc/>
        public override ItemType ItemType
        {
            get => base.ItemType;
            set
            {
                if (value != ItemType.SCP330)
                    throw new ArgumentOutOfRangeException(nameof(Type), value, "ItemType must be ItemType.SCP330");

                base.ItemType = value;
            }
        }

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
        public virtual TextDisplay EatenCustomCandyMessage { get; set; } = new("You have eaten a custom candy. Let's see what effect you will get...", 5, true, TextChannelType.Hint);

        /// <summary>
        /// Gets or sets a <see cref="TextDisplay"/> that will be displayed when player has received custom candy.
        /// </summary>
        public virtual TextDisplay ReceiveCustomCandyMessage { get; set; } = new("You have received a custom candy!", 5, true, TextChannelType.Hint);

        /// <inheritdoc/>
        public override TextDisplay SelectedText { get; set; }

        /// <summary>
        /// Applies effect to player.
        /// </summary>
        /// <param name="player">Player to apply effects.</param>
        public virtual void ApplyEffects(Pawn player)
        {
        }
    }
}