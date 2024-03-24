// -----------------------------------------------------------------------
// <copyright file="ActiveAbilityBehaviour.cs" company="Exiled Team">
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
    public abstract class ActiveAbilityBehaviour : ActiveAbilityBehaviour<Item>
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

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(ActiveAbilitySettings.Activated);
        }

        /// <inheritdoc/>
        protected override void OnExpired()
        {
            base.OnActivated();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(ActiveAbilitySettings.Expired);
        }

        /// <inheritdoc/>
        protected override void OnReady()
        {
            base.OnReady();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(ActiveAbilitySettings.OnReady);
        }
    }
}