// -----------------------------------------------------------------------
// <copyright file="PassiveAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PlayerAbilities
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a player, providing support for passive usage.
    /// </summary>
    public abstract class PassiveAbilityBehaviour : PassiveAbilityBehaviour<Player>
    {
        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Player.Get(Base);
    }
}