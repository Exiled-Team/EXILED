// -----------------------------------------------------------------------
// <copyright file="UnlockableAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PlayerAbilities
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a player, providing support for levels and unlocking the ability.
    /// </summary>
    public abstract class UnlockableAbilityBehaviour : UnlockableAbilityBehaviour<Player>
    {
        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Player.Get(Base);

        /// <inheritdoc/>
        protected override void OnUnlocked()
        {
            base.OnUnlocked();

            Owner.ShowHint(Settings.Unlocked);
        }

        /// <inheritdoc/>
        protected override void DenyActivation()
        {
            base.DenyActivation();

            Owner.ShowHint(Settings.CannotBeUsed);
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            Owner.ShowHint(Settings.Activated);
        }

        /// <inheritdoc/>
        protected override void OnExpired()
        {
            base.OnActivated();

            Owner.ShowHint(Settings.Expired);
        }

        /// <inheritdoc/>
        protected override void OnLevelAdded()
        {
            base.OnLevelAdded();

            Owner.ShowHint(Settings.NextLevel);
        }

        /// <inheritdoc/>
        protected override void OnLevelRemoved()
        {
            base.OnLevelRemoved();

            Owner.ShowHint(Settings.PreviousLevel);
        }

        /// <inheritdoc/>
        protected override void OnMaxLevelReached()
        {
            base.OnMaxLevelReached();

            Owner.ShowHint(Settings.MaxLevelReached);
        }
    }
}