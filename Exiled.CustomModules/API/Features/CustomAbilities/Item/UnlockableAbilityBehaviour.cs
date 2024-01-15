// -----------------------------------------------------------------------
// <copyright file="UnlockableAbilityBehaviour.cs" company="Exiled Team">
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
    /// Represents the base class for ability behaviors associated with an item, providing support for levels and unlocking the ability.
    /// </summary>
    public abstract class UnlockableAbilityBehaviour : UnlockableAbilityBehaviour<Item>
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
        protected override void OnUnlocked()
        {
            base.OnUnlocked();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.Unlocked);
        }

        /// <inheritdoc/>
        protected override void DenyActivation()
        {
            base.DenyActivation();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.CannotBeUsed);
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.Activated);
        }

        /// <inheritdoc/>
        protected override void OnExpired()
        {
            base.OnActivated();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.Expired);
        }

        /// <inheritdoc/>
        protected override void OnReady()
        {
            base.OnReady();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.OnReady);
        }

        /// <inheritdoc/>
        protected override void OnLevelAdded()
        {
            base.OnLevelAdded();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.NextLevel);
        }

        /// <inheritdoc/>
        protected override void OnLevelRemoved()
        {
            base.OnLevelRemoved();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.PreviousLevel);
        }

        /// <inheritdoc/>
        protected override void OnMaxLevelReached()
        {
            base.OnMaxLevelReached();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(UnlockableAbilitySettings.MaxLevelReached);
        }
    }
}