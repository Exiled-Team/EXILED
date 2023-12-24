// -----------------------------------------------------------------------
// <copyright file="CustomPickupAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using System;

    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Pickups;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// Represents a base class for custom abilities associated with a specific <see cref="Pickup"/>.
    /// </summary>
    public abstract class CustomPickupAbility : CustomAbility<Pickup>
    {
        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all registered custom abilities.
        /// </summary>
        public static new IEnumerable<CustomPickupAbility> List => Registered[typeof(Pickup)].Cast<CustomPickupAbility>();

        /// <summary>
        /// Gets all owners and all their respective <see cref="CustomPickupAbility"/>'s.
        /// </summary>
        public static new Dictionary<Pickup, HashSet<CustomPickupAbility>> Manager =>
            CustomAbility<Pickup>.Manager.Where(kvp => kvp.Key is Pickup)
            .ToDictionary(kvp => (Pickup)kvp.Key, kvp => kvp.Value.Cast<CustomPickupAbility>().ToHashSet());

        /// <summary>
        /// Gets all owners belonging to a <see cref="CustomPickupAbility"/>.
        /// </summary>
        public static IEnumerable<Pickup> Owners => Manager.Keys.ToHashSet();

        /// <summary>
        /// Gets a <see cref="CustomPickupAbility"/> given the specified <paramref name="customAbilityType"/>.
        /// </summary>
        /// <param name="customAbilityType">The specified ability type.</param>
        /// <returns>The <see cref="CustomPickupAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new CustomPickupAbility Get(object customAbilityType) =>
            List.FirstOrDefault(customAbility => customAbility == customAbilityType && customAbility.IsEnabled);

        /// <summary>
        /// Gets a <see cref="CustomPickupAbility"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomPickupAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new CustomPickupAbility Get(string name) => List.FirstOrDefault(customAbility => customAbility.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomPickupAbility"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomPickupAbility"/> matching the search or <see langword="null"/> if not found.</returns>
        public static new CustomPickupAbility Get(Type type) =>
            (type.BaseType != typeof(IAbilityBehaviour) && !type.IsSubclassOf(typeof(IAbilityBehaviour))) ? null :
            List.FirstOrDefault(customAbility => customAbility.BehaviourComponent == type);

        /// <summary>
        /// Gets all <see cref="CustomPickupAbility"/>'s from a <see cref="Pickup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CustomPickupAbility"/>'s owner.</param>
        /// <returns>The <see cref="CustomPickupAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new IEnumerable<CustomPickupAbility> Get(Pickup entity) => Manager.FirstOrDefault(kvp => kvp.Key == entity).Value;

        /// <summary>
        /// Tries to get a <see cref="CustomPickupAbility"/> given the specified <paramref name="customAbility"/>.
        /// </summary>
        /// <param name="customAbilityType">The <see cref="object"/> to look for.</param>
        /// <param name="customAbility">The found <paramref name="customAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <paramref name="customAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object customAbilityType, out CustomPickupAbility customAbility) => customAbility = Get(customAbilityType);

        /// <summary>
        /// Tries to get a <paramref name="customAbility"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomPickupAbility"/> name to look for.</param>
        /// <param name="customAbility">The found <see cref="CustomPickupAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomPickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomPickupAbility customAbility) => customAbility = List.FirstOrDefault(cAbility => cAbility.Name == name);

        /// <summary>
        /// Tries to get the pickup's current <see cref="CustomPickupAbility"/>'s.
        /// </summary>
        /// <param name="entity">The entity to search on.</param>
        /// <param name="customAbility">The found <see cref="CustomPickupAbility"/>'s, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomPickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pickup entity, out IEnumerable<CustomPickupAbility> customAbility) => (customAbility = Get(entity)) is not null;

        /// <summary>
        /// Tries to get the pickup's current <see cref="CustomPickupAbility"/>.
        /// </summary>
        /// <param name="abilityBehaviour">The <see cref="IAbilityBehaviour"/> to search for.</param>
        /// <param name="customAbility">The found <see cref="CustomPickupAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomPickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(IAbilityBehaviour abilityBehaviour, out CustomPickupAbility customAbility) => customAbility = Get(abilityBehaviour.GetType());

        /// <summary>
        /// Tries to get the pickup's current <see cref="CustomPickupAbility"/>.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <param name="customAbility">The found <see cref="CustomPickupAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomPickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomPickupAbility customAbility) => customAbility = Get(type.GetType());

        /// <inheritdoc cref="CustomAbility{T}.Add{TAbility}(T, out TAbility)"/>
        public static new bool Add<TAbility>(Pickup entity, out TAbility param)
            where TAbility : CustomPickupAbility => CustomAbility<Pickup>.Add(entity, out param);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, Type)"/>
        public static new bool Add(Pickup entity, Type type) => CustomAbility<Pickup>.Add(entity, type);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, string)"/>
        public static new bool Add(Pickup entity, string name) => CustomAbility<Pickup>.Add(entity, name);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, uint)"/>
        public static new bool Add(Pickup entity, uint id) => CustomAbility<Pickup>.Add(entity, id);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{Type})"/>
        public static new void Add(Pickup entity, IEnumerable<Type> types) => CustomAbility<Pickup>.Add(entity, types);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{string})"/>
        public static new void Add(Pickup entity, IEnumerable<string> names) => CustomAbility<Pickup>.Add(entity, names);

        /// <inheritdoc cref="CustomAbility{T}.Remove{TAbility}(T)"/>
        public static new bool Remove<TAbility>(Pickup entity)
            where TAbility : CustomPickupAbility => CustomAbility<Pickup>.Remove<TAbility>(entity);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, Type)"/>
        public static new bool Remove(Pickup entity, Type type) => CustomAbility<Pickup>.Remove(entity, type);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, string)"/>
        public static new bool Remove(Pickup entity, string name) => CustomAbility<Pickup>.Remove(entity, name);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, string)"/>
        public static new bool Remove(Pickup entity, uint id) => CustomAbility<Pickup>.Remove(entity, id);

        /// <inheritdoc cref="CustomAbility{T}.RemoveAll(T)"/>
        public static new void RemoveAll(Pickup entity) => CustomAbility<Pickup>.RemoveAll(entity);

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{Type})"/>
        public static new void RemoveRange(Pickup entity, IEnumerable<Type> types) => CustomAbility<Pickup>.RemoveRange(entity, types);

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{string})"/>
        public static new void RemoveRange(Pickup entity, IEnumerable<string> names) => CustomAbility<Pickup>.RemoveRange(entity, names);

        /// <inheritdoc cref="CustomAbility{T}.Add(T)"/>
        public new void Add(Pickup entity) => base.Add(entity);

        /// <inheritdoc cref="CustomAbility{T}.Remove(T)"/>
        public new bool Remove(Pickup entity) => base.Remove(entity);
    }
}