// -----------------------------------------------------------------------
// <copyright file="ItemAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.ItemAbilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Core;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents a base class for custom abilities associated with a specific <see cref="Item"/>.
    /// </summary>
    public abstract class ItemAbility : CustomAbility<Item>
    {
        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all registered custom abilities.
        /// </summary>
        public static new IEnumerable<ItemAbility> List => Registered[typeof(Item)].Cast<ItemAbility>();

        /// <summary>
        /// Gets all owners and all their respective <see cref="ItemAbility"/>'s.
        /// </summary>
        public static new Dictionary<Item, HashSet<ItemAbility>> Manager =>
            CustomAbility<Item>.Manager.Where(kvp => kvp.Key is Item)
            .ToDictionary(kvp => (Item)kvp.Key, kvp => kvp.Value.Cast<ItemAbility>().ToHashSet());

        /// <summary>
        /// Gets all owners belonging to a <see cref="ItemAbility"/>.
        /// </summary>
        public static IEnumerable<Item> Owners => Manager.Keys.ToHashSet();

        /// <summary>
        /// Gets a <see cref="ItemAbility"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="ItemAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new ItemAbility Get(uint id) => CustomAbility<Item>.Get(id).Cast<ItemAbility>();

        /// <summary>
        /// Gets a <see cref="ItemAbility"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="ItemAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new ItemAbility Get(string name) => CustomAbility<Item>.Get(name).Cast<ItemAbility>();

        /// <summary>
        /// Gets a <see cref="ItemAbility"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="ItemAbility"/> matching the search or <see langword="null"/> if not found.</returns>
        public static new ItemAbility Get(Type type) => CustomAbility<Item>.Get(type).Cast<ItemAbility>();

        /// <summary>
        /// Gets all <see cref="ItemAbility"/>'s from a <see cref="Item"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ItemAbility"/>'s owner.</param>
        /// <returns>The <see cref="ItemAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new IEnumerable<ItemAbility> Get(Item entity) => CustomAbility<Item>.Get(entity).Cast<ItemAbility>();

        /// <summary>
        /// Tries to get a <see cref="ItemAbility"/> given the specified <paramref name="customAbility"/>.
        /// </summary>
        /// <param name="id">Theid to look for.</param>
        /// <param name="customAbility">The found <paramref name="customAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <paramref name="customAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out ItemAbility customAbility) => customAbility = Get(id);

        /// <summary>
        /// Tries to get a <paramref name="customAbility"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="ItemAbility"/> name to look for.</param>
        /// <param name="customAbility">The found <see cref="ItemAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="ItemAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out ItemAbility customAbility) => customAbility = List.FirstOrDefault(cAbility => cAbility.Name == name);

        /// <summary>
        /// Tries to get the item's current abilities.
        /// </summary>
        /// <param name="entity">The entity to search on.</param>
        /// <param name="customAbilities">The found <see cref="ItemAbility"/>'s, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="ItemAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Item entity, out IEnumerable<ItemAbility> customAbilities) => (customAbilities = Get(entity)) is not null;

        /// <summary>
        /// Tries to get the item's current <see cref="ItemAbility"/>.
        /// </summary>
        /// <param name="abilityBehaviour">The <see cref="IAbilityBehaviour"/> to search for.</param>
        /// <param name="customAbility">The found <see cref="ItemAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="ItemAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(IAbilityBehaviour abilityBehaviour, out ItemAbility customAbility) => customAbility = Get(abilityBehaviour.GetType());

        /// <summary>
        /// Tries to get the item's current <see cref="ItemAbility"/>.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <param name="customAbility">The found <see cref="ItemAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="ItemAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out ItemAbility customAbility) => customAbility = Get(type.GetType());

        /// <inheritdoc cref="CustomAbility{T}.Add{TAbility}(T, out TAbility)"/>
        public static new bool Add<TAbility>(Item entity, out TAbility param)
            where TAbility : ItemAbility => CustomAbility<Item>.Add(entity, out param);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, Type)"/>
        public static new bool Add(Item entity, Type type) => CustomAbility<Item>.Add(entity, type) && StaticActor.Get<AbilityTracker>().AddOrTrack(entity);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, string)"/>
        public static new bool Add(Item entity, string name) => CustomAbility<Item>.Add(entity, name) && StaticActor.Get<AbilityTracker>().AddOrTrack(entity);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, uint)"/>
        public static new bool Add(Item entity, uint id) => CustomAbility<Item>.Add(entity, id) && StaticActor.Get<AbilityTracker>().AddOrTrack(entity);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{Type})"/>
        public static new void Add(Item entity, IEnumerable<Type> types)
        {
            CustomAbility<Item>.Add(entity, types);
            StaticActor.Get<AbilityTracker>().AddOrTrack(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{string})"/>
        public static new void Add(Item entity, IEnumerable<string> names)
        {
            CustomAbility<Item>.Add(entity, names);
            StaticActor.Get<AbilityTracker>().AddOrTrack(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove{TAbility}(T)"/>
        public static new bool Remove<TAbility>(Item entity)
            where TAbility : ItemAbility => Remove(entity, typeof(TAbility));

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, Type)"/>
        public static new bool Remove(Item entity, Type type)
        {
            if (!TryGet(entity, out IEnumerable<ItemAbility> customAbilities))
                return false;

            ItemAbility itemAbility = customAbilities.FirstOrDefault(ability => ability.GetType() == type);
            if (!entity.TryGetComponent(itemAbility.BehaviourComponent, out EActor component) || component is not IAbilityBehaviour behaviour)
                return false;

            StaticActor.Get<AbilityTracker>().Remove(entity, behaviour);
            return CustomAbility<Item>.Remove(entity, type);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, string)"/>
        public static new bool Remove(Item entity, string name)
        {
            if (!TryGet(name, out ItemAbility itemAbility) ||
                !entity.TryGetComponent(itemAbility.BehaviourComponent, out EActor component) ||
                component is not IAbilityBehaviour behaviour)
                return false;

            StaticActor.Get<AbilityTracker>().Remove(entity, behaviour);
            return CustomAbility<Item>.Remove(entity, name);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, uint)"/>
        public static new bool Remove(Item entity, uint id)
        {
            if (!TryGet(id, out ItemAbility itemAbility) ||
                !entity.TryGetComponent(itemAbility.BehaviourComponent, out EActor component) ||
                component is not IAbilityBehaviour behaviour)
                return false;

            StaticActor.Get<AbilityTracker>().Remove(entity, behaviour);
            return CustomAbility<Item>.Remove(entity, id);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveAll(T)"/>
        public static new void RemoveAll(Item entity)
        {
            StaticActor.Get<AbilityTracker>().Remove(entity);
            CustomAbility<Item>.RemoveAll(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{Type})"/>
        public static new void RemoveRange(Item entity, IEnumerable<Type> types)
        {
            foreach (Type t in types)
                Remove(entity, t);

            CustomAbility<Item>.RemoveRange(entity, types);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{string})"/>
        public static new void RemoveRange(Item entity, IEnumerable<string> names)
        {
            foreach (string name in names)
                Remove(entity, name);

            CustomAbility<Item>.RemoveRange(entity, names);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{uint})"/>
        public static new void RemoveRange(Item entity, IEnumerable<uint> ids)
        {
            foreach (uint id in ids)
                Remove(entity, id);

            CustomAbility<Item>.RemoveRange(entity, ids);
        }

        /// <inheritdoc cref="CustomAbility{T}.Add(T)"/>
        public new void Add(Item entity)
        {
            base.Add(entity);

            StaticActor.Get<AbilityTracker>().AddOrTrack(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove(T)"/>
        public new bool Remove(Item entity)
        {
            if (!entity.TryGetComponent(BehaviourComponent, out EActor component) || component is not IAbilityBehaviour behaviour)
                return false;

            StaticActor.Get<AbilityTracker>().Remove(entity, behaviour);

            return base.Remove(entity);
        }
    }
}