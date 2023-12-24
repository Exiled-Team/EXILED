// -----------------------------------------------------------------------
// <copyright file="AbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PickupAbilities
{
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for pickup-specific ability behaviors.
    /// </summary>
    /// <typeparam name="TSettings">The type of settings associated with the pickup-specific ability behavior.</typeparam>
    public abstract class AbilityBehaviour<TSettings> : AbilityBehaviourBase<Pickup, TSettings>
        where TSettings : AbilitySettings
    {
    }
}