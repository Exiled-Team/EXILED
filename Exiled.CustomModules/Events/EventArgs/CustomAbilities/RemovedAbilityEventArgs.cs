// -----------------------------------------------------------------------
// <copyright file="RemovedAbilityEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomAbilities
{
    using Exiled.API.Features.Core;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Contains all information after removing an ability.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="GameEntity"/>.</typeparam>
    public class RemovedAbilityEventArgs<T>
        where T : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedAbilityEventArgs{T}"/> class.
        /// </summary>
        /// <param name="entity"><inheritdoc cref="Entity"/></param>
        /// <param name="ability"><inheritdoc cref="Ability"/></param>
        public RemovedAbilityEventArgs(T entity, CustomAbility<T> ability)
        {
            Entity = entity;
            Ability = ability;
        }

        /// <summary>
        /// Gets the removed custom ability.
        /// </summary>
        public CustomAbility<T> Ability { get; }

        /// <summary>
        /// Gets the entity to which the custom ability has been removed.
        /// </summary>
        public T Entity { get; }
    }
}