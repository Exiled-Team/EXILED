// -----------------------------------------------------------------------
// <copyright file="LevelAbilityBehaviour.cs" company="Exiled Team">
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
    /// Represents the base class for ability behaviors associated with an item, providing support for levels.
    /// </summary>
    public abstract class LevelAbilityBehaviour : LevelAbilityBehaviour<Item>
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
                ItemOwner.ShowTextDisplay(LevelAbilitySettings.Activated);
        }

        /// <inheritdoc/>
        protected override void OnExpired()
        {
            base.OnActivated();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(LevelAbilitySettings.Expired);
        }

        /// <inheritdoc/>
        protected override void OnReady()
        {
            base.OnReady();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(LevelAbilitySettings.OnReady);
        }

        /// <inheritdoc/>
        protected override void OnLevelAdded()
        {
            base.OnLevelAdded();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(LevelAbilitySettings.NextLevel);
        }

        /// <inheritdoc/>
        protected override void OnLevelRemoved()
        {
            base.OnLevelRemoved();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(LevelAbilitySettings.PreviousLevel);
        }

        /// <inheritdoc/>
        protected override void OnMaxLevelReached()
        {
            base.OnMaxLevelReached();

            if (ItemOwner)
                ItemOwner.ShowTextDisplay(LevelAbilitySettings.MaxLevelReached);
        }
    }
}