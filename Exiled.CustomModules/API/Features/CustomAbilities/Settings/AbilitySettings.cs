// -----------------------------------------------------------------------
// <copyright file="AbilitySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities.Settings
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.CustomRoles;

    /// <summary>
    /// Represents the base class for player-specific ability behaviors.
    /// </summary>
    public class AbilitySettings : TypeCastObject<AbilitySettings>, IAdditiveProperty
    {
        /// <summary>
        /// Gets the default <see cref="AbilitySettings"/> values.
        /// </summary>
        public static AbilitySettings Default { get; } = new();
    }
}