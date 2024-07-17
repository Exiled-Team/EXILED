// -----------------------------------------------------------------------
// <copyright file="PassiveAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using Exiled.API.Features.Core;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a specific entity type, providing support for passive usage.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity associated with the ability behavior.</typeparam>
    public abstract class PassiveAbilityBehaviour<TEntity> : AbilityBehaviourBase<TEntity>
        where TEntity : GameEntity
    {
    }
}