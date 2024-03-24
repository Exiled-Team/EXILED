// -----------------------------------------------------------------------
// <copyright file="AmmoBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Pickups.Ammos
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.CustomModules.API.Features.CustomItems.Pickups;

    /// <summary>
    /// Represents the base class for custom ammo behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ItemBehaviour"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game ammo.
    /// </remarks>
    public abstract class AmmoBehaviour : PickupBehaviour
    {
        /// <inheritdoc cref="ItemBehaviour.Settings"/>.
        public AmmoSettings AmmoSettings => Settings.Cast<AmmoSettings>();

        /// <summary>
        /// Gets or sets the box size.
        /// </summary>
        public ushort BoxSize { get; set; }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            BoxSize = AmmoSettings.BoxSizes.Random();
        }

        /// <inheritdoc/>
        protected override void OnAcquired(Player player, bool displayMessage = true)
        {
            base.OnAcquired(player, displayMessage);

            player.Cast<Pawn>().AddAmmo(CustomItem.Id, BoxSize);
        }
    }
}