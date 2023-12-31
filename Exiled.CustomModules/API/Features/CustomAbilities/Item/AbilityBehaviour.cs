// -----------------------------------------------------------------------
// <copyright file="AbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.ItemAbilities
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for item-specific ability behaviors.
    /// </summary>
    public abstract class AbilityBehaviour : ActiveAbilityBehaviour<Item>
    {
        /// <inheritdoc/>
        public override bool DisposeOnNullOwner { get; protected set; } = false;

        /// <inheritdoc/>
        public override bool IsReady => base.IsReady && ItemOwner && ItemOwner.CurrentItem == Owner;

        /// <summary>
        /// Gets the item's owner.
        /// </summary>
        public Player ItemOwner => Owner.Owner;

        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Item.Get(Base);

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            ItemOwner?.ShowHint(Settings.Activated);
        }

        /// <inheritdoc/>
        protected override void OnExpired()
        {
            base.OnActivated();

            ItemOwner?.ShowHint(Settings.Expired);
        }

        /// <inheritdoc/>
        protected override void OnReady()
        {
            base.OnReady();

            ItemOwner?.ShowHint(Settings.OnReady);
        }
    }
}