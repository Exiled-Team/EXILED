// -----------------------------------------------------------------------
// <copyright file="AbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.ItemAbilities
{
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for item-specific ability behaviors.
    /// </summary>
    /// <typeparam name="TSettings">The type of settings associated with the item-specific ability behavior.</typeparam>
    public abstract class AbilityBehaviour<TSettings> : AbilityBehaviourBase<Item, TSettings>
        where TSettings : AbilitySettings
    {
    }
}