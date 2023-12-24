// -----------------------------------------------------------------------
// <copyright file="AbilityBehaviour.cs" company="Exiled Team">
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
    /// <typeparam name="TSettings">The type of settings associated with the player-specific ability behavior.</typeparam>
    public abstract class AbilityBehaviour<TSettings> : AbilityBehaviourBase<Player, TSettings>
        where TSettings : AbilitySettings
    {
    }
}