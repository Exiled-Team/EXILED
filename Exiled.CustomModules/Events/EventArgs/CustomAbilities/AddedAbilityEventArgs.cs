// -----------------------------------------------------------------------
// <copyright file="AddedAbilityEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomAbilities
{
    using Exiled.API.Features.Core;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after adding an ability.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="GameEntity"/>.</typeparam>
    public class AddedAbilityEventArgs<T>
        where T : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddedAbilityEventArgs{T}"/> class.
        /// </summary>
        /// <param name="entity"><inheritdoc cref="Entity"/></param>
        /// <param name="ability"><inheritdoc cref="Ability"/></param>
        public AddedAbilityEventArgs(T entity, CustomAbility<T> ability)
        {
            Entity = entity;
            Ability = ability;
        }

        /// <summary>
        /// Gets the added custom ability.
        /// </summary>
        public CustomAbility<T> Ability { get; }

        /// <summary>
        /// Gets the entity to which the custom ability has been added.
        /// </summary>
        public T Entity { get; }
    }
}