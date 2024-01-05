// -----------------------------------------------------------------------
// <copyright file="CustomAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using System;

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Features.CustomEscapes;
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
    public abstract class CustomAbility<T> : TypeCastObject<CustomAbility<T>>, IAdditiveBehaviour
        where T : GameEntity
    {
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

        /// <inheritdoc/>
        public Type BehaviourComponent { get; protected set; }

        /// <summary>
        /// Gets the <see cref="CustomAbility{T}"/>'s name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomAbility{T}"/>'s id.
        /// </summary>
        public virtual uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the ability is enabled.
        /// </summary>
        public virtual bool IsEnabled => true;

        /// <summary>
        /// Gets the reflected generic type.
        /// </summary>
        protected Type ReflectedGenericType => reflectedGenericType ??= GetType().GetGenericArguments()[0];

        /// <summary>
        /// Compares two operands: <see cref="CustomAbility{T}"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomAbility{T}"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomAbility<T> left, object right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomAbility{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator ==(object left, CustomAbility<T> right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="CustomAbility{T}"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomAbility{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomAbility<T> left, object right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="object"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomAbility{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomAbility<T> right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="CustomAbility{T}"/> and <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomAbility{T}"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomAbility{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomAbility<T> left, CustomAbility<T> right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Compares two operands: <see cref="CustomAbility{T}"/> and <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomAbility{T}"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomAbility{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomAbility<T> left, CustomAbility<T> right) => !(left.Id == right.Id);

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomAbility<T> Get(uint id) =>
            UnorderedList.FirstOrDefault(customAbility => customAbility == id && customAbility.IsEnabled);

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomAbility<T> Get(string name) => UnorderedList.FirstOrDefault(customAbility => customAbility.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified <see cref="Type"/> of an <see cref="IAbilityBehaviour"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomAbility<T> Get(Type type) => (type.BaseType != typeof(IAbilityBehaviour) && !type.IsSubclassOf(typeof(IAbilityBehaviour))) ? null :
            UnorderedList.FirstOrDefault(customAbility => customAbility.BehaviourComponent == type);

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified <see cref="Type"/> of <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <typeparam name="TAbility">The specified <see cref="Type"/>.</typeparam>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not found.</returns>
        public static TAbility Get<TAbility>()
            where TAbility : CustomAbility<T> => UnorderedList.FirstOrDefault(customAbility => customAbility.GetType() == typeof(TAbility)).Cast<TAbility>();

        /// <summary>
        /// Gets a <see cref="CustomAbility{T}"/> given the specified <see cref="IAbilityBehaviour"/>.
        /// </summary>
        /// <param name="abilityBehaviour">The specified <see cref="IAbilityBehaviour"/>.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomAbility<T> Get(IAbilityBehaviour abilityBehaviour) => Get(abilityBehaviour.GetType());

        /// <summary>
        /// Gets all <see cref="CustomAbility{T}"/>'s from a <see cref="Player"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CustomAbility{T}"/>'s owner.</param>
        /// <returns>The <see cref="CustomAbility{T}"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static IEnumerable<CustomAbility<T>> Get(T entity) => EntitiesValue.FirstOrDefault(kvp => kvp.Key == entity).Value;

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
        public static bool TryGet(string name, out CustomAbility<T> customAbility) => customAbility = UnorderedList.FirstOrDefault(cAbility => cAbility.Name == name);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomAbility{T}"/>'s.
        /// </summary>
        /// <param name="entity">The entity to search on.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility{T}"/>'s, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(T entity, out IEnumerable<CustomAbility<T>> customAbility) => (customAbility = Get(entity)) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="CustomAbility{T}"/>.
        /// </summary>
        /// <param name="abilityBehaviour">The <see cref="IAbilityBehaviour"/> to search for.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility{T}"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(IAbilityBehaviour abilityBehaviour, out CustomAbility<T> customAbility) => customAbility = Get(abilityBehaviour.GetType());

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
            where TAbility : CustomAbility<T> => (customAbility = Get<TAbility>()) is not null;

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
            where TAbility : CustomAbility<T> => TryGet(out TAbility customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, entity.GameObject);

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
            TryGet(type, out CustomAbility<T> customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, entity.GameObject);

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
            TryGet(name, out CustomAbility<T> customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, entity.GameObject);

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
            TryGet(id, out CustomAbility<T> customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, entity.GameObject);

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
        public static IEnumerable<CustomAbility<T>> EnableAll()
        {
            List<CustomAbility<T>> customAbilities = new();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                CustomAbilityAttribute attribute = type.GetCustomAttribute<CustomAbilityAttribute>();
                if (!type.IsSubclassOf(typeof(CustomAbility<T>)) || attribute is null)
                    continue;

                CustomAbility<T> customAbility = Activator.CreateInstance(type) as CustomAbility<T>;

                if (!customAbility.IsEnabled)
                    continue;

                if (customAbility.TryRegister(attribute))
                    customAbilities.Add(customAbility);
            }

            Log.SendRaw($"{customAbilities.Count()} custom abilities have been successfully registered!", ConsoleColor.DarkGreen);

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

            Log.SendRaw($"{customAbilities.Count()} custom abilities have been successfully unregistered!", ConsoleColor.DarkGreen);

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
            entity.AddComponent(BehaviourComponent);

            try
            {
                EntitiesValue[entity].Add(this);
            }
            catch
            {
                EntitiesValue.Add(entity, new HashSet<CustomAbility<T>>() { this });
            }
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
                if (Get(entity).Count() == 1)
                    EntitiesValue.Remove(entity);
                else
                    EntitiesValue[entity].Remove(this);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the provided id is equal to the current object.
        /// </summary>
        /// <param name="id">The id to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(uint id) => Id == id;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="ca">The custom ability to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomAbility<T> ca) => ca && (ReferenceEquals(this, ca) || Id == ca.Id);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (Equals(obj as CustomAbility<T>))
                return true;

            try
            {
                return Equals((uint)obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();

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

                if (UnorderedRegistered.Any(customAbility => customAbility.Name == Name))
                {
                    Log.Debug(
                        $"Couldn't register {Name}. Another custom ability has been registered with the same Type: " +
                        $" {UnorderedRegistered.FirstOrDefault(customAbility => customAbility.Name == Name)}");

                    return false;
                }

                EObject.RegisterObjectType(BehaviourComponent, Name);
                UnorderedRegistered.Add(this);

                if (!Registered.ContainsKey(ReflectedGenericType))
                    Registered[ReflectedGenericType] = new();

                Registered[ReflectedGenericType].Add(this);

                return true;
            }

            Log.Debug(
                $"Couldn't register {Name}." +
                $"This custom ability has been already registered.");

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
                Log.Debug(
                    $"[Couldn't unregister {Name}." +
                    $"This custom ability hasn't been registered yet.");

                return false;
            }

            EObject.UnregisterObjectType(Name);
            UnorderedRegistered.Remove(this);
            Registered[ReflectedGenericType].Remove(this);

            return true;
        }
    }
}