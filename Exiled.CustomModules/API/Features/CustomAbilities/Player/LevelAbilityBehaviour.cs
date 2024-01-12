// -----------------------------------------------------------------------
// <copyright file="LevelAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PlayerAbilities
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a player, providing support for levels.
    /// </summary>
    public abstract class LevelAbilityBehaviour : LevelAbilityBehaviour<Player>, ISelectableAbility
    {
        /// <inheritdoc/>
        public virtual bool IsSelectable => true;

        /// <inheritdoc/>
        public virtual bool IsSelected => Owner is Pawn pawn && pawn.SelectedAbilityBehaviour && pawn.SelectedAbilityBehaviour == this;

        /// <inheritdoc/>
        public virtual void Select() => Owner.Cast<Pawn>().SelectedAbilityBehaviour = this;

        /// <inheritdoc/>
        public virtual void Unselect()
        {
            if (!IsSelected)
                return;

            Owner.Cast<Pawn>().SelectedAbilityBehaviour = null;
        }

        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Player.Get(Base);

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            Owner.ShowTextDisplay(LevelAbilitySettings.Activated);
        }

        /// <inheritdoc/>
        protected override void OnExpired()
        {
            base.OnActivated();

            Owner.ShowTextDisplay(LevelAbilitySettings.Expired);
        }

        /// <inheritdoc/>
        protected override void OnReady()
        {
            base.OnReady();

            Owner.ShowTextDisplay(LevelAbilitySettings.OnReady);
        }

        /// <inheritdoc/>
        protected override void OnLevelAdded()
        {
            base.OnLevelAdded();

            Owner.ShowTextDisplay(LevelAbilitySettings.NextLevel);
        }

        /// <inheritdoc/>
        protected override void OnLevelRemoved()
        {
            base.OnLevelRemoved();

            Owner.ShowTextDisplay(LevelAbilitySettings.PreviousLevel);
        }

        /// <inheritdoc/>
        protected override void OnMaxLevelReached()
        {
            base.OnMaxLevelReached();

            Owner.ShowTextDisplay(LevelAbilitySettings.MaxLevelReached);
        }
    }
}