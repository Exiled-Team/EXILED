// -----------------------------------------------------------------------
// <copyright file="ActiveAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PlayerAbilities
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for player-specific ability behaviors.
    /// </summary>
    public abstract class ActiveAbilityBehaviour : ActiveAbilityBehaviour<Player>, ISelectableAbility
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
        protected override void PostInitialize()
        {
            base.PostInitialize();
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            Owner.ShowTextDisplay(ActiveAbilitySettings.Activated);
        }

        /// <inheritdoc/>
        protected override void OnExpired()
        {
            base.OnActivated();

            Owner.ShowTextDisplay(ActiveAbilitySettings.Expired);
        }

        /// <inheritdoc/>
        protected override void OnReady()
        {
            base.OnReady();

            Owner.ShowTextDisplay(ActiveAbilitySettings.OnReady);
        }
    }
}