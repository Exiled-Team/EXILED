// -----------------------------------------------------------------------
// <copyright file="CustomAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.CustomModules.API.Enums;

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomAbilities.Settings;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.Events.EventArgs.CustomAbilities;
    using HarmonyLib;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// Abstract base class serving as a foundation for custom abilities associated with a specific <see cref="GameEntity"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="GameEntity"/> associated with the custom ability.</typeparam>
    /// <remarks>
    /// The <see cref="CustomAbility{T}"/> class establishes a flexible structure for creating and managing custom abilities tied to a particular <see cref="GameEntity"/>.
    /// <para>
    /// This class is parameterized by the type <typeparamref name="T"/> to denote the specific <see cref="GameEntity"/> associated with the custom ability.
    /// <br/>It is designed to be utilized in conjunction with the <see cref="IAdditiveBehaviour"/> interface, enabling seamless integration into existing systems for extending and enhancing ability-related functionalities.
    /// </para>
    /// </remarks>
    public abstract class CustomAbility<T> : CustomModule, ICustomAbility
        where T : GameEntity
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        protected static readonly Dictionary<Type, CustomAbility<T>> TypeLookupTable = new();
        protected static readonly Dictionary<Type, CustomAbility<T>> BehaviourLookupTable = new();
        protected static readonly Dictionary<uint, CustomAbility<T>> IdLookupTable = new();
        protected static readonly Dictionary<string, CustomAbility<T>> NameLookupTable = new();
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private Type reflectedGenericType;

        /// <inheritdoc cref="Manager"/>
#pragma warning disable SA1202 // Elements should be ordered by access
        internal static readonly Dictionary<object, HashSet<CustomAbility<T>>> EntitiesValue = new();
#pragma warning restore SA1202 // Elements should be ordered by access

        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="CustomAbility{T}"/> containing all the registered custom abilites.
        /// </summary>
        protected static readonly List<CustomAbility<T>> UnorderedRegistered = new();

        /// <summary>
        /// A <see cref="Dictionary{TKey, TValue}"/> containing all the registered custom abilites ordered by their <see cref="GameEntity"/> type.
        /// </summary>
        protected static readonly Dictionary<Type, HashSet<CustomAbility<T>>> Registered = new();

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="CustomAbility{T}"/> containing all the registered custom abilites.
        /// </summary>
        public static IReadOnlyDictionary<Type, HashSet<CustomAbility<T>>> List => Registered;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility{T}"/> containing all the registered custom abilites.
        /// </summary>
        public static IEnumerable<CustomAbility<T>> UnorderedList => UnorderedRegistered;

        /// <summary>
        /// Gets all entities and all their respective <see cref="CustomAbility{T}"/>'s.
        /// </summary>
        public static IReadOnlyDictionary<object, HashSet<CustomAbility<T>>> Manager => EntitiesValue;

        /// <summary>
        /// Gets all entities belonging to a <see cref="CustomAbility{T}"/>.
        /// </summary>
        public static HashSet<object> Entities => EntitiesValue.Keys.ToHashSet();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before adding an ability.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<AddingAbilityEventArgs<T>> AddingAbilityDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after adding an ability.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<AddedAbilityEventArgs<T>> AddedAbilityDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before removing an ability.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<RemovingAbilityEventArgs<T>> RemovingAbilityDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after removing an ability.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<RemovedAbilityEventArgs<T>> RemovedAbilityDispatcher { get; protected set; }

        /// <inheritdoc/>
        public Type BehaviourComponent { get; protected set; }

        /// <summary>
        /// Gets the ability's settings.
        /// </summary>
        public virtual AbilitySettings Settings { get; } = AbilitySettings.Default;

        /// <summary>
        /// Gets the <see cref="CustomAbility{T}"/>'s name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomAbility{T}"/>'s id.
        /// </summary>
        public override uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the ability is enabled.
        /// </summary>
        public override bool IsEnabled { get; }

        /// <summary>
        /// Gets the description of the ability.
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// Gets the reflected generic type.
        /// </summary>
        protected Type ReflectedGenericType => reflectedGenericType ??= GetType().GetGenericArguments()[0];

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> based on the provided id or <see cref="UUCustomAbilityType"/>.
        /// </summary>
        /// <param name="id">The id or <see cref="UUCustomAbilityType"/> of the custom ability.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> with the specified id, or <see langword="null"/> if no ability is found.</returns>
        public static CustomAbility<T> Get(object id) => id is uint or UUCustomAbilityType ? Get((uint)id) : null;

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomAbility<T> Get(uint id) => IdLookupTable[id];

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomAbility<T> Get(string name) => NameLookupTable[name];

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomAbility<T> Get(Type type) =>
            typeof(IAbilityBehaviour).IsAssignableFrom(type) ? BehaviourLookupTable[type] :
            typeof(CustomAbility<T>).IsAssignableFrom(type) ? TypeLookupTable[type] : null;

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified <see cref="Type"/> of <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <typeparam name="TAbility">The specified <see cref="Type"/>.</typeparam>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not found.</returns>
        public static TAbility Get<TAbility>()
            where TAbility : CustomAbility<T> => TypeLookupTable[typeof(TAbility)].Cast<TAbility>();

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified <see cref="IAbilityBehaviour"/>.
        /// </summary>
        /// <param name="abilityBehaviour">The specified <see cref="IAbilityBehaviour"/>.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomAbility<T> Get(IAbilityBehaviour abilityBehaviour) => Get(abilityBehaviour.GetType());

        /// <summary>
        /// Gets all <see cref="CustomAbility{T}"/> instances from a <see cref="Player"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CustomAbility{T}"/>'s owner.</param>
        /// <returns>All <see cref="CustomAbility{T}"/> instances matching the search.</returns>
        public static IEnumerable<CustomAbility<T>> Get(T entity) => EntitiesValue.TryGetValue(entity, out HashSet<CustomAbility<T>> abilities) ? abilities : Enumerable.Empty<CustomAbility<T>>();

        /// <summary>
        /// Tries to get a <see cref="CustomAbility{T}"/> given the specified id.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <param name="customAbility">The found <paramref name="customAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <paramref name="customAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out CustomAbility<T> customAbility) => customAbility = Get(id);

        /// <summary>
        /// Tries to get a <paramref name="customAbility"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomAbility{T}"/> name to look for.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility{T}"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomAbility<T> customAbility) => customAbility = Get(name);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomAbility{T}"/>'s.
        /// </summary>
        /// <param name="entity">The entity to search on.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility{T}"/>'s, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(T entity, out IEnumerable<CustomAbility<T>> customAbility) => (customAbility = Get(entity)) is not null;

        /// <summary>
        /// Tries to get a <see cref="CustomAbility{T}"/> given the specified type of <see cref="IAbilityBehaviour"/>.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility{T}"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomAbility<T> customAbility) => customAbility = Get(type.GetType());

        /// <summary>
        /// Tries to get a <see cref="CustomAbility{T}"/> given the specified type of <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <typeparam name="TAbility">The type to search for.</typeparam>
        /// <param name="customAbility">The found <see cref="CustomAbility{T}"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet<TAbility>(out TAbility customAbility)
            where TAbility : CustomAbility<T> => customAbility = Get<TAbility>();

        /// <summary>
        /// Gets the <see cref="EActor"/> associated with the specified custom ability type for the given entity.
        /// </summary>
        /// <typeparam name="TAbility">The type of the custom ability.</typeparam>
        /// <param name="entity">The entity for which the custom ability is associated.</param>
        /// <returns>The <see cref="EActor"/> associated with the specified custom ability type for the given entity.</returns>
        /// <remarks>
        /// This method retrieves the <see cref="EActor"/> associated with the specified custom ability type for the given entity.
        /// It is typically used to access the behavior component associated with a specific custom ability for an entity.
        /// </remarks>
        public static EActor Get<TAbility>(T entity)
            where TAbility : CustomAbility<T> => entity.GetComponent(Get<TAbility>().BehaviourComponent);

        /// <summary>
        /// Adds the specified custom ability to the given entity and returns it.
        /// </summary>
        /// <typeparam name="TAbility">The type of the custom ability.</typeparam>
        /// <param name="entity">The entity to which the custom ability will be added.</param>
        /// <param name="param">The added custom ability instance.</param>
        /// <returns><see langword="true"/> if the custom ability was added successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method attempts to add the specified custom ability to the given entity. If the addition is successful,
        /// it returns the added custom ability instance. If the addition fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Add<TAbility>(T entity, out TAbility param)
            where TAbility : CustomAbility<T>
        {
            param = null;

            if (!TryGet(out TAbility ability))
                return false;

            (param = ability.Cast<TAbility>()).Add(entity);
            return true;
        }

        /// <summary>
        /// Adds the custom ability of the specified type to the given entity.
        /// </summary>
        /// <param name="entity">The entity to which the custom ability will be added.</param>
        /// <param name="type">The type of the custom ability.</param>
        /// <returns><see langword="true"/> if the custom ability was added successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method attempts to add the custom ability of the specified type to the given entity.
        /// If the addition is successful, it returns <see langword="true"/>. If the addition fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Add(T entity, Type type)
        {
            if (!TryGet(type, out CustomAbility<T> customAbility))
                return false;

            customAbility.Add(entity);
            return true;
        }

        /// <summary>
        /// Adds the custom ability of the specified type to the given entity.
        /// </summary>
        /// <param name="entity">The entity to which the custom ability will be added.</param>
        /// <param name="type">The type name of the custom ability.</param>
        /// <returns><see langword="true"/> if the custom ability was added successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method attempts to add the custom ability of the specified type to the given entity.
        /// If the addition is successful, it returns <see langword="true"/>. If the addition fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Add(T entity, string type)
        {
            if (!TryGet(type, out CustomAbility<T> customAbility))
                return false;

            customAbility.Add(entity);
            return true;
        }

        /// <summary>
        /// Adds the custom ability identified by the specified ID to the given entity.
        /// </summary>
        /// <param name="entity">The entity to which the custom ability will be added.</param>
        /// <param name="id">The ID of the custom ability.</param>
        /// <returns><see langword="true"/> if the custom ability was added successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method attempts to add the custom ability identified by the specified ID to the given entity.
        /// If the addition is successful, it returns <see langword="true"/>. If the addition fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Add(T entity, uint id)
        {
            if (!TryGet(id, out CustomAbility<T> customAbility))
                return false;

            customAbility.Add(entity);
            return true;
        }

        /// <summary>
        /// Adds multiple custom abilities of the specified types to the given entity.
        /// </summary>
        /// <param name="entity">The entity to which the custom abilities will be added.</param>
        /// <param name="types">The types of the custom abilities to be added.</param>
        /// <remarks>
        /// This method adds multiple custom abilities of the specified types to the given entity.
        /// </remarks>
        public static void Add(T entity, IEnumerable<Type> types)
        {
            foreach (Type type in types)
                Add(entity, type);
        }

        /// <summary>
        /// Adds multiple custom abilities identified by their type names to the given entity.
        /// </summary>
        /// <param name="entity">The entity to which the custom abilities will be added.</param>
        /// <param name="types">The type names of the custom abilities to be added.</param>
        /// <remarks>
        /// This method adds multiple custom abilities identified by their type names to the given entity.
        /// </remarks>
        public static void Add(T entity, IEnumerable<string> types)
        {
            foreach (string type in types)
                Add(entity, type);
        }

        /// <summary>
        /// Removes the custom ability of type <typeparamref name="TAbility"/> from the specified entity.
        /// </summary>
        /// <typeparam name="TAbility">The type of custom ability to be removed.</typeparam>
        /// <param name="entity">The entity from which the custom ability will be removed.</param>
        /// <returns><see langword="true"/> if the custom ability was removed successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method removes the custom ability of the specified type from the specified entity. If the removal operation fails,
        /// the method returns <see langword="false"/>. The removal process involves destroying the active object associated with the custom ability.
        /// </remarks>
        public static bool Remove<TAbility>(T entity)
            where TAbility : CustomAbility<T> => TryGet(out TAbility customAbility) && customAbility.Remove(entity);

        /// <summary>
        /// Removes the custom ability of the specified type from the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which the custom ability will be removed.</param>
        /// <param name="type">The type of custom ability to be removed.</param>
        /// <returns><see langword="true"/> if the custom ability was removed successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method removes the custom ability of the specified type from the specified entity. If the removal operation fails,
        /// the method returns <see langword="false"/>. The removal process involves destroying the active object associated with the custom ability.
        /// </remarks>
        public static bool Remove(T entity, Type type) =>
            TryGet(type, out CustomAbility<T> customAbility) && customAbility.Remove(entity);

        /// <summary>
        /// Removes the custom ability of the specified name from the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which the custom ability will be removed.</param>
        /// <param name="name">The name of the custom ability type to be removed.</param>
        /// <returns><see langword="true"/> if the custom ability was removed successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method removes the custom ability of the specified type from the specified entity. If the removal operation fails,
        /// the method returns <see langword="false"/>. The removal process involves destroying the active object associated with the custom ability.
        /// </remarks>
        public static bool Remove(T entity, string name) =>
            TryGet(name, out CustomAbility<T> customAbility) && customAbility.Remove(entity);

        /// <summary>
        /// Removes the custom ability with the specified ID from the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which the custom ability will be removed.</param>
        /// <param name="id">The ID of the custom ability to be removed.</param>
        /// <returns><see langword="true"/> if the custom ability was removed successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method removes the custom ability with the specified ID from the specified entity. If the removal operation fails,
        /// the method returns <see langword="false"/>. The removal process involves destroying the active object associated with the custom ability.
        /// </remarks>
        public static bool Remove(T entity, uint id) =>
            TryGet(id, out CustomAbility<T> customAbility) && customAbility.Remove(entity);

        /// <summary>
        /// Removes all custom abilities associated with the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which all custom abilities will be removed.</param>
        /// <remarks>
        /// This method removes all custom abilities associated with the specified entity. The removal process involves destroying the active objects
        /// associated with each custom ability. If the entity has no custom abilities, the method has no effect.
        /// </remarks>
        public static void RemoveAll(T entity)
        {
            if (EntitiesValue.TryGetValue(entity, out HashSet<CustomAbility<T>> abilities))
                abilities.ForEach(customAbility => customAbility.Remove(entity));
        }

        /// <summary>
        /// Removes custom abilities of the specified types from the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which custom abilities will be removed.</param>
        /// <param name="types">The types of custom abilities to be removed.</param>
        /// <remarks>
        /// This method removes custom abilities of the specified types from the specified entity. The removal process involves destroying the active objects
        /// associated with each custom ability. If the entity has no custom abilities of the specified types, the method has no effect.
        /// </remarks>
        public static void RemoveRange(T entity, IEnumerable<Type> types)
        {
            if (EntitiesValue.TryGetValue(entity, out HashSet<CustomAbility<T>> abilities))
                abilities.DoIf(ability => types.Contains(ability.GetType()), ability => ability.Remove(entity));
        }

        /// <summary>
        /// Removes custom abilities with the specified names from the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which custom abilities will be removed.</param>
        /// <param name="names">The names of custom abilities to be removed.</param>
        /// <remarks>
        /// This method removes custom abilities with the specified names from the specified entity. The removal process involves destroying the active objects
        /// associated with each custom ability. If the entity has no custom abilities with the specified names, the method has no effect.
        /// </remarks>
        public static void RemoveRange(T entity, IEnumerable<string> names)
        {
            if (EntitiesValue.TryGetValue(entity, out HashSet<CustomAbility<T>> abilities))
                abilities.DoIf(ability => names.Contains(ability.Name), ability => ability.Remove(entity));
        }

        /// <summary>
        /// Removes custom abilities with the specified ids from the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which custom abilities will be removed.</param>
        /// <param name="ids">The ids of custom abilities to be removed.</param>
        /// <remarks>
        /// This method removes custom abilities with the specified ids from the specified entity. The removal process involves destroying the active objects
        /// associated with each custom ability. If the entity has no custom abilities with the specified ids, the method has no effect.
        /// </remarks>
        public static void RemoveRange(T entity, IEnumerable<uint> ids)
        {
            if (Manager.TryGetValue(entity, out HashSet<CustomAbility<T>> abilities))
                abilities.DoIf(ability => ids.Contains(ability.Id), ability => ability.Remove(entity));
        }

        /// <summary>
        /// Enables all the custom abilities present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility{T}"/> which contains all the enabled custom abilities.</returns>
        public static IEnumerable<CustomAbility<T>> EnableAll() => EnableAll(Assembly.GetCallingAssembly());

        /// <summary>
        /// Enables all the custom abilities present in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to enable the abilities from.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility{T}"/> which contains all the enabled custom abilities.</returns>
        public static IEnumerable<CustomAbility<T>> EnableAll(Assembly assembly)
        {
            List<CustomAbility<T>> customAbilities = new();
            foreach (Type type in assembly.GetTypes())
            {
                CustomAbilityAttribute attribute = type.GetCustomAttribute<CustomAbilityAttribute>();
                if (!typeof(CustomAbility<T>).IsAssignableFrom(type) || attribute is null)
                    continue;

                CustomAbility<T> customAbility = Activator.CreateInstance(type) as CustomAbility<T>;

                if (!customAbility.IsEnabled)
                    continue;

                if (customAbility.TryRegister(attribute))
                    customAbilities.Add(customAbility);
            }

            if (customAbilities.Count != Registered.Count)
                Log.Info($"{customAbilities.Count()} custom abilities have been successfully registered!");

            return customAbilities;
        }

        /// <summary>
        /// Disables all the custom abilities present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility{T}"/> which contains all the disabled custom abilities.</returns>
        public static IEnumerable<CustomAbility<T>> DisableAll()
        {
            List<CustomAbility<T>> customAbilities = new();
            customAbilities.AddRange(UnorderedRegistered.Where(ability => ability.TryUnregister()));

            Log.Info($"{customAbilities.Count()} custom abilities have been successfully unregistered!");

            return customAbilities;
        }

        /// <summary>
        /// Adds the custom ability to the specified entity.
        /// </summary>
        /// <param name="entity">The entity to which the custom ability will be added.</param>
        /// <remarks>
        /// This method adds the custom ability to the specified entity, incorporating its behavior into the entity's functionality.
        /// If the entity already has a custom ability of the same type, the method ensures that the entity is associated with the new instance.
        /// </remarks>
        public void Add(T entity)
        {
            AddingAbilityEventArgs<T> inEv = new(entity, this);
            AddingAbilityDispatcher.InvokeAll(inEv);

            if (!inEv.IsAllowed)
                return;

            entity.AddComponent(BehaviourComponent);

            if (!EntitiesValue.TryAdd(entity, new HashSet<CustomAbility<T>> { this }))
                EntitiesValue[entity].Add(this);

            AddedAbilityEventArgs<T> outEv = new(entity, this);
            AddedAbilityDispatcher.InvokeAll(outEv);
        }

        /// <summary>
        /// Removes the custom ability from the specified entity.
        /// </summary>
        /// <param name="entity">The entity from which the custom ability will be removed.</param>
        /// <returns><see langword="true"/> if the custom ability was removed successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method removes the custom ability from the specified entity. If the entity has multiple instances of the same custom ability,
        /// it ensures that only the association with the current instance is removed. If the entity has no remaining instances of the custom ability,
        /// the method removes the entity entry from the internal storage.
        /// </remarks>
        public bool Remove(T entity)
        {
            try
            {
                RemovingAbilityEventArgs<T> inEv = new(entity, this);
                RemovingAbilityDispatcher.InvokeAll(inEv);

                if (!inEv.IsAllowed)
                    return false;

                if (Get(entity).Count() == 1)
                    EntitiesValue.Remove(entity);
                else
                    EntitiesValue[entity].Remove(this);

                if (entity.TryGetComponent(BehaviourComponent, out EActor component) && !component.IsDestroying)
                    component.Destroy();

                RemovedAbilityEventArgs<T> outEv = new(entity, this);
                RemovedAbilityDispatcher.InvokeAll(outEv);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to register a <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomAbilityAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility{T}"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomAbilityAttribute attribute = null)
        {
            if (!UnorderedRegistered.Contains(this))
            {
                if (attribute is not null && Id == 0)
                    Id = attribute.Id;

                CustomAbility<T> duplicate = UnorderedRegistered.FirstOrDefault(x => x.Id == Id || x.Name == Name || x.BehaviourComponent == BehaviourComponent);
                if (duplicate)
                {
                    Log.Warn($"Unable to register {Name}. Another ability with the same ID, Name or Behaviour Component already exists: {duplicate.Name}");

                    return false;
                }

                EObject.RegisterObjectType(BehaviourComponent, Name);
                UnorderedRegistered.Add(this);

                TypeLookupTable.TryAdd(GetType(), this);
                BehaviourLookupTable.TryAdd(BehaviourComponent, this);
                IdLookupTable.TryAdd(Id, this);
                NameLookupTable.TryAdd(Name, this);

                if (!Registered.ContainsKey(ReflectedGenericType))
                    Registered[ReflectedGenericType] = new();

                Registered[ReflectedGenericType].Add(this);

                return true;
            }

            Log.Warn($"Unable to register {Name}. Ability already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility{T}"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!UnorderedList.Contains(this))
            {
                Log.Warn($"Unable to unregister {Name}. Ability is not yet registered.");

                return false;
            }

            EObject.UnregisterObjectType(Name);
            UnorderedRegistered.Remove(this);
            Registered[ReflectedGenericType].Remove(this);
            TypeLookupTable.Remove(GetType());
            BehaviourLookupTable.Remove(BehaviourComponent);
            IdLookupTable.Remove(Id);
            NameLookupTable.Remove(Name);

            return true;
        }
    }
}