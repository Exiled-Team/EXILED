// -----------------------------------------------------------------------
// <copyright file="BaseCandy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using Exiled.API.Features;
    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// A class that convert custom candy to base game candy.
    /// </summary>
    internal class BaseCandy : ICandy
    {
        private CandySettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCandy"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="CandySettings"/> that will be encapsulated.</param>
        internal BaseCandy(CandySettings settings)
        {
            this.settings = settings;
        }

        /// <inheritdoc/>
        public CandyKindID Kind => settings.CandyType;

        /// <inheritdoc/>
        public float SpawnChanceWeight => settings.Weight;

        /// <inheritdoc/>
        public void ServerApplyEffects(ReferenceHub hub) => settings.ApplyEffects(Player.Get(hub).Cast<Pawn>());
    }
}