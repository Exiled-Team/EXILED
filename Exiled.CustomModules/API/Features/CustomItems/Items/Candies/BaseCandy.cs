// -----------------------------------------------------------------------
// <copyright file="BaseCandy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using System;

    using Exiled.API.Features;
    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// A class that converts custom candy to base game candy.
    /// </summary>
    internal class BaseCandy : ICandy
    {
        private CandySettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCandy"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="CandySettings"/> that will be encapsulated.</param>
        /// <param name="applyEffectsDelegate">The delegate to be passed to <see cref="ServerApplyEffects"/>.</param>
        internal BaseCandy(CandySettings settings, Action<Pawn> applyEffectsDelegate)
        {
            this.settings = settings;
        }

        /// <inheritdoc/>
        public CandyKindID Kind => settings.CandyType;

        /// <inheritdoc/>
        public float SpawnChanceWeight => settings.Weight / 100;

        /// <summary>
        /// Gets the delegate to be passed to <see cref="ServerApplyEffects"/>.
        /// </summary>
        public Action<Pawn> ApplyEffects { get; }

        /// <inheritdoc/>
        public void ServerApplyEffects(ReferenceHub hub) => ApplyEffects(Player.Get(hub).Cast<Pawn>());
    }
}