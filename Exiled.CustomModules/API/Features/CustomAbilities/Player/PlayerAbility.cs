// -----------------------------------------------------------------------
// <copyright file="PlayerAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PlayerAbilities
{
    using System;

    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    using Utils.NonAllocLINQ;

    /// <summary>
    /// Represents a base class for custom abilities associated with a specific <see cref="Player"/>.
    /// </summary>
    public abstract class PlayerAbility : CustomAbility<Player>
    {
        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all registered custom abilities.
        /// </summary>
        public static new IEnumerable<PlayerAbility> List => Registered[typeof(Player)].Cast<PlayerAbility>();

        /// <summary>
        /// Gets all owners and all their respective <see cref="PlayerAbility"/>'s.
        /// </summary>
        public static new Dictionary<Player, HashSet<PlayerAbility>> Manager =>
            CustomAbility<Player>.Manager.Where(kvp => kvp.Key is Player)
            .ToDictionary(kvp => (Player)kvp.Key, kvp => kvp.Value.Cast<PlayerAbility>().ToHashSet());

        /// <summary>
        /// Gets all owners belonging to a <see cref="PlayerAbility"/>.
        /// </summary>
        public static IEnumerable<Player> Owners => Manager.Keys.ToHashSet();

        /// <summary>
        /// Gets a <see cref="PlayerAbility"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="PlayerAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new PlayerAbility Get(uint id) =>
            List.FirstOrDefault(customAbility => customAbility == id && customAbility.IsEnabled);

        /// <summary>
        /// Gets a <see cref="PlayerAbility"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="PlayerAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new PlayerAbility Get(string name) => List.FirstOrDefault(customAbility => customAbility.Name == name);

        /// <summary>
        /// Gets a <see cref="PlayerAbility"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="PlayerAbility"/> matching the search or <see langword="null"/> if not found.</returns>
        public static new PlayerAbility Get(Type type) =>
            type.BaseType != typeof(IAbilityBehaviour) && !type.IsSubclassOf(typeof(IAbilityBehaviour)) ? null :
            List.FirstOrDefault(customAbility => customAbility.BehaviourComponent == type);

        /// <summary>
        /// Gets all <see cref="PlayerAbility"/>'s from a <see cref="Player"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PlayerAbility"/>'s owner.</param>
        /// <returns>The <see cref="PlayerAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new IEnumerable<PlayerAbility> Get(Player entity) => Manager.FirstOrDefault(kvp => kvp.Key == entity).Value;

        /// <summary>
        /// Tries to get a <see cref="PlayerAbility"/> given the specified <paramref name="customAbility"/>.
        /// </summary>
        /// <param name="customAbilityType">The <see cref="object"/> to look for.</param>
        /// <param name="customAbility">The found <paramref name="customAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <paramref name="customAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint customAbilityType, out PlayerAbility customAbility) => customAbility = Get(customAbilityType);

        /// <summary>
        /// Tries to get a <paramref name="customAbility"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="PlayerAbility"/> name to look for.</param>
        /// <param name="customAbility">The found <see cref="PlayerAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PlayerAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out PlayerAbility customAbility) => customAbility = List.FirstOrDefault(cAbility => cAbility.Name == name);

        /// <summary>
        /// Tries to get the player's current <see cref="PlayerAbility"/>'s.
        /// </summary>
        /// <param name="entity">The entity to search on.</param>
        /// <param name="customAbility">The found <see cref="PlayerAbility"/>'s, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PlayerAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Player entity, out IEnumerable<PlayerAbility> customAbility) => (customAbility = Get(entity)) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="PlayerAbility"/>.
        /// </summary>
        /// <param name="abilityBehaviour">The <see cref="IAbilityBehaviour"/> to search for.</param>
        /// <param name="customAbility">The found <see cref="PlayerAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PlayerAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(IAbilityBehaviour abilityBehaviour, out PlayerAbility customAbility) => customAbility = Get(abilityBehaviour.GetType());

        /// <summary>
        /// Tries to get the player's current <see cref="PlayerAbility"/>.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <param name="customAbility">The found <see cref="PlayerAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PlayerAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out PlayerAbility customAbility) => customAbility = Get(type.GetType());

        /// <inheritdoc cref="CustomAbility{T}.Add{TAbility}(T, out TAbility)"/>
        public static new bool Add<TAbility>(Player entity, out TAbility param)
            where TAbility : PlayerAbility => CustomAbility<Player>.Add(entity, out param);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, Type)"/>
        public static new bool Add(Player entity, Type type) => CustomAbility<Player>.Add(entity, type);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, string)"/>
        public static new bool Add(Player entity, string name) => CustomAbility<Player>.Add(entity, name);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, uint)"/>
        public static new bool Add(Player entity, uint id) => CustomAbility<Player>.Add(entity, id);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{Type})"/>
        public static new void Add(Player entity, IEnumerable<Type> types) => CustomAbility<Player>.Add(entity, types);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{string})"/>
        public static new void Add(Player entity, IEnumerable<string> names) => CustomAbility<Player>.Add(entity, names);

        /// <inheritdoc cref="CustomAbility{T}.Remove{TAbility}(T)"/>
        public static new bool Remove<TAbility>(Player entity)
            where TAbility : PlayerAbility => CustomAbility<Player>.Remove<TAbility>(entity);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, Type)"/>
        public static new bool Remove(Player entity, Type type) => CustomAbility<Player>.Remove(entity, type);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, string)"/>
        public static new bool Remove(Player entity, string name) => CustomAbility<Player>.Remove(entity, name);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, string)"/>
        public static new bool Remove(Player entity, uint id) => CustomAbility<Player>.Remove(entity, id);

        /// <inheritdoc cref="CustomAbility{T}.RemoveAll(T)"/>
        public static new void RemoveAll(Player entity) => CustomAbility<Player>.RemoveAll(entity);

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{Type})"/>
        public static new void RemoveRange(Player entity, IEnumerable<Type> types) => CustomAbility<Player>.RemoveRange(entity, types);

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{string})"/>
        public static new void RemoveRange(Player entity, IEnumerable<string> names) => CustomAbility<Player>.RemoveRange(entity, names);

        /// <inheritdoc cref="CustomAbility{T}.Add(T)"/>
        public new void Add(Player entity) => base.Add(entity);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T)"/>
        public new bool Remove(Player entity) => base.Remove(entity);
    }
}