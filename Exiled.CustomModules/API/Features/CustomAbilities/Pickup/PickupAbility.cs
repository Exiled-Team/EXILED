// -----------------------------------------------------------------------
// <copyright file="PickupAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PickupAbilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Core;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents a base class for custom abilities associated with a specific <see cref="Pickup"/>.
    /// </summary>
    public abstract class PickupAbility : CustomAbility<Pickup>
    {
        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all registered custom abilities.
        /// </summary>
        public static new IEnumerable<PickupAbility> List => Registered[typeof(Pickup)].Cast<PickupAbility>();

        /// <summary>
        /// Gets all owners and all their respective <see cref="PickupAbility"/>'s.
        /// </summary>
        public static new Dictionary<Pickup, HashSet<PickupAbility>> Manager =>
            CustomAbility<Pickup>.Manager.Where(kvp => kvp.Key is Pickup)
            .ToDictionary(kvp => (Pickup)kvp.Key, kvp => kvp.Value.Cast<PickupAbility>().ToHashSet());

        /// <summary>
        /// Gets all owners belonging to a <see cref="PickupAbility"/>.
        /// </summary>
        public static IEnumerable<Pickup> Owners => Manager.Keys.ToHashSet();

        /// <summary>
        /// Gets the <see cref="TrackerBase"/>.
        /// </summary>
        protected static TrackerBase Tracker { get; } = TrackerBase.Get();

        /// <summary>
        /// Gets a <see cref="PickupAbility"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="PickupAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new PickupAbility Get(uint id) => CustomAbility<Pickup>.Get(id).Cast<PickupAbility>();

        /// <summary>
        /// Gets a <see cref="PickupAbility"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="PickupAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new PickupAbility Get(string name) => CustomAbility<Pickup>.Get(name).Cast<PickupAbility>();

        /// <summary>
        /// Gets a <see cref="PickupAbility"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="PickupAbility"/> matching the search or <see langword="null"/> if not found.</returns>
        public static new PickupAbility Get(Type type) => CustomAbility<Pickup>.Get(type).Cast<PickupAbility>();

        /// <summary>
        /// Gets all <see cref="PickupAbility"/>'s from a <see cref="Pickup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PickupAbility"/>'s owner.</param>
        /// <returns>The <see cref="PickupAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static new IEnumerable<PickupAbility> Get(Pickup entity) => CustomAbility<Pickup>.Get(entity).Cast<PickupAbility>();

        /// <summary>
        /// Tries to get a <see cref="PickupAbility"/> given the specified id.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <param name="customAbility">The found <paramref name="customAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <paramref name="customAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out PickupAbility customAbility) => customAbility = Get(id);

        /// <summary>
        /// Tries to get a <paramref name="customAbility"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="PickupAbility"/> name to look for.</param>
        /// <param name="customAbility">The found <see cref="PickupAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out PickupAbility customAbility) => customAbility = Get(name);

        /// <summary>
        /// Tries to get the pickup's current <see cref="PickupAbility"/>'s.
        /// </summary>
        /// <param name="entity">The entity to search on.</param>
        /// <param name="customAbility">The found <see cref="PickupAbility"/>'s, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pickup entity, out IEnumerable<PickupAbility> customAbility) => (customAbility = Get(entity)) is not null;

        /// <summary>
        /// Tries to get the pickup's current <see cref="PickupAbility"/>.
        /// </summary>
        /// <param name="abilityBehaviour">The <see cref="IAbilityBehaviour"/> to search for.</param>
        /// <param name="customAbility">The found <see cref="PickupAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(IAbilityBehaviour abilityBehaviour, out PickupAbility customAbility) => customAbility = Get(abilityBehaviour.GetType());

        /// <summary>
        /// Tries to get the pickup's current <see cref="PickupAbility"/>.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <param name="customAbility">The found <see cref="PickupAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="PickupAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out PickupAbility customAbility) => customAbility = Get(type);

        /// <inheritdoc cref="CustomAbility{T}.Add{TAbility}(T, out TAbility)"/>
        public static new bool Add<TAbility>(Pickup entity, out TAbility param)
            where TAbility : PickupAbility => CustomAbility<Pickup>.Add(entity, out param);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, Type)"/>
        public static new bool Add(Pickup entity, Type type) => CustomAbility<Pickup>.Add(entity, type) && Tracker.AddOrTrack(entity);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, string)"/>
        public static new bool Add(Pickup entity, string name) => CustomAbility<Pickup>.Add(entity, name) && Tracker.AddOrTrack(entity);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, uint)"/>
        public static new bool Add(Pickup entity, uint id) => CustomAbility<Pickup>.Add(entity, id) && Tracker.AddOrTrack(entity);

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{Type})"/>
        public static new void Add(Pickup entity, IEnumerable<Type> types)
        {
            CustomAbility<Pickup>.Add(entity, types);
            Tracker.AddOrTrack(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.Add(T, IEnumerable{string})"/>
        public static new void Add(Pickup entity, IEnumerable<string> names)
        {
            CustomAbility<Pickup>.Add(entity, names);
            Tracker.AddOrTrack(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove{TAbility}(T)"/>
        public static new bool Remove<TAbility>(Pickup entity)
            where TAbility : PickupAbility => Remove(entity, typeof(TAbility));

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, Type)"/>
        public static new bool Remove(Pickup entity, Type type)
        {
            if (!TryGet(entity, out IEnumerable<PickupAbility> customAbilities))
                return false;

            PickupAbility pickupAbility = customAbilities.FirstOrDefault(ability => ability.GetType() == type);
            if (!entity.TryGetComponent(pickupAbility.BehaviourComponent, out EActor component) || component is not IAbilityBehaviour behaviour)
                return false;

            Tracker.Remove(entity, behaviour);
            return CustomAbility<Pickup>.Remove(entity, type);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, string)"/>
        public static new bool Remove(Pickup entity, string name)
        {
            if (!TryGet(name, out PickupAbility pickupAbility) ||
                !entity.TryGetComponent(pickupAbility.BehaviourComponent, out EActor component) ||
                component is not IAbilityBehaviour behaviour)
                return false;

            Tracker.Remove(entity, behaviour);
            return CustomAbility<Pickup>.Remove(entity, name);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove(T, uint)"/>
        public static new bool Remove(Pickup entity, uint id)
        {
            if (!TryGet(id, out PickupAbility pickupAbility) ||
                !entity.TryGetComponent(pickupAbility.BehaviourComponent, out EActor component) ||
                component is not IAbilityBehaviour behaviour)
                return false;

            Tracker.Remove(entity, behaviour);
            return CustomAbility<Pickup>.Remove(entity, id);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveAll(T)"/>
        public static new void RemoveAll(Pickup entity)
        {
            Tracker.Remove(entity);
            CustomAbility<Pickup>.RemoveAll(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{Type})"/>
        public static new void RemoveRange(Pickup entity, IEnumerable<Type> types)
        {
            foreach (Type t in types)
                Remove(entity, t);

            CustomAbility<Pickup>.RemoveRange(entity, types);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{string})"/>
        public static new void RemoveRange(Pickup entity, IEnumerable<string> names)
        {
            foreach (string name in names)
                Remove(entity, name);

            CustomAbility<Pickup>.RemoveRange(entity, names);
        }

        /// <inheritdoc cref="CustomAbility{T}.RemoveRange(T, IEnumerable{uint})"/>
        public static new void RemoveRange(Pickup entity, IEnumerable<uint> ids)
        {
            foreach (uint id in ids)
                Remove(entity, id);

            CustomAbility<Pickup>.RemoveRange(entity, ids);
        }

        /// <inheritdoc cref="CustomAbility{T}.Add(T)"/>
        public new void Add(Pickup entity)
        {
            base.Add(entity);

            Tracker.AddOrTrack(entity);
        }

        /// <inheritdoc cref="CustomAbility{T}.Remove(T)"/>
        public new bool Remove(Pickup entity)
        {
            if (!entity.TryGetComponent(BehaviourComponent, out EActor component) || component is not IAbilityBehaviour behaviour)
                return false;

            Tracker.Remove(entity, behaviour);

            return base.Remove(entity);
        }
    }
}