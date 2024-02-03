// -----------------------------------------------------------------------
// <copyright file="CustomGameMode.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomGameModes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.Attributes;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// Represents a custom game mode in the system, derived from <see cref="CustomModule"/> and implementing <see cref="IAdditiveBehaviours"/>.
    /// This class serves as a base for creating custom game modes within the system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Custom game modes extend functionality through inheritance from this base class and can implement additional behaviors
    /// by also implementing the <see cref="IAdditiveBehaviours"/> interface.
    /// </para>
    /// <para>
    /// Game modes encapsulate specific rules, mechanics, and behaviors that define the gameplay experience within the system.
    /// </para>
    /// <para>
    /// Additionally, there are two important classes associated with custom game modes:
    /// <list type="bullet">
    ///     <item>
    ///         <term><see cref="GameState"/></term>
    ///         <description>
    ///         Represents the state of the game on the server. It defines rules and conditions specific to the custom game mode during the round,
    ///         including win conditions and other relevant settings.
    ///         <br/>
    ///         The <see cref="GameState"/> class is used to encapsulate the current state of the game, such as in-progress rounds,
    ///         outcomes, or any other state-related information relevant to the custom game mode.
    ///         <br/>
    ///         Classes derived from <see cref="GameState"/> are responsible for defining and enforcing the rules and conditions of the custom game mode on the server.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="PlayerState"/></term>
    ///         <description>
    ///         Represents the state of an individual player within the custom game mode.
    ///         <br/>
    ///         The <see cref="PlayerState"/> class manages the behavior and interactions of a single player within the custom game mode. It governs the specific actions of a single player.
    ///         <br/>
    ///         It is responsible for handling player-specific actions and responses, providing a way to customize the experience for each player.
    ///         </description>
    ///     </item>
    /// </list>
    /// </para>
    /// </remarks>
    public abstract class CustomGameMode : CustomModule, IAdditiveBehaviours
    {
        private static readonly List<CustomGameMode> Registered = new();
        private static readonly Dictionary<Type, CustomGameMode> TypeLookupTable = new();
        private static readonly Dictionary<Type[], CustomGameMode> BehavioursLookupTable = new();
        private static readonly Dictionary<uint, CustomGameMode> IdLookupTable = new();
        private static readonly Dictionary<string, CustomGameMode> NameLookupTable = new();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all <see cref="CustomGameMode"/>'s.
        /// </summary>
        public static IEnumerable<CustomGameMode> List => Registered;

        /// <inheritdoc/>
        public override string Name { get; }

        /// <inheritdoc/>
        public override uint Id { get; protected set; }

        /// <inheritdoc/>
        public override bool IsEnabled { get; }

        /// <inheritdoc/>
        public virtual Type[] BehaviourComponents { get; }

        /// <summary>
        /// Gets the <see cref="GameModeSettings"/>.
        /// </summary>
        public virtual GameModeSettings Settings { get; }

        /// <summary>
        /// Gets a value indicating whether the game mode can start automatically based on the configured probability, if automatic.
        /// </summary>
        /// <returns><see langword="true"/> if the game mode can start automatically; otherwise, <see langword="false"/>.</returns>
        public bool CanStartAuto => Settings.Automatic && Settings.AutomaticProbability.EvaluateProbability();

        /// <summary>
        /// Gets the type of the game state.
        /// </summary>
        /// <param name="customGameMode">The custom game mode.</param>
        /// <returns>The type of the game state if found; otherwise, <see langword="null"/>.</returns>
        public Type GameState => BehaviourComponents.FirstOrDefault(comp => typeof(GameState).IsAssignableFrom(comp));

        /// <summary>
        /// Gets the types of the player states.
        /// </summary>
        /// <param name="customGameMode">The custom game mode.</param>
        /// <returns>The types of the player states if found; otherwise, empty.</returns>
        public IEnumerable<Type> PlayerStates => BehaviourComponents.Where(comp => typeof(PlayerState).IsAssignableFrom(comp));

        /// <summary>
        /// Gets all <see cref="CustomGameMode"/> instances based on the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>All <see cref="CustomGameMode"/> instances matching the predicate.</returns>
        public static IEnumerable<CustomGameMode> Get(Func<CustomGameMode, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets a <see cref="CustomGameMode"/> based on the provided id or <see cref="UUGameModeType"/>.
        /// </summary>
        /// <param name="id">The id or <see cref="UUGameModeType"/> of the custom game mode.</param>
        /// <returns>The <see cref="CustomGameMode"/> with the specified id, or <see langword="null"/> if no game mode is found.</returns>
        public static CustomGameMode Get(object id) => id is uint or UUGameModeType ? Get((uint)id) : null;

        /// <summary>
        /// Gets a <see cref="CustomGameMode"/> given the specified <see cref="Id"/>.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="CustomGameMode"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomGameMode Get(uint id) => IdLookupTable[id];

        /// <summary>
        /// Gets a <see cref="CustomGameMode"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomGameMode"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomGameMode Get(string name) => NameLookupTable[name];

        /// <summary>
        /// Gets a <see cref="CustomGameMode"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomGameMode"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomGameMode Get(Type type) =>
            typeof(GameState).IsAssignableFrom(type) ? BehavioursLookupTable.FirstOrDefault(kvp => kvp.Key.Any(t => t == type)).Value :
            typeof(CustomGameMode).IsAssignableFrom(type) ? TypeLookupTable[type] : null;

        /// <summary>
        /// Gets a <see cref="CustomGameMode"/> given the specified <see cref="GameState"/>.
        /// </summary>
        /// <param name="gameState">The specified <see cref="GameState"/>.</param>
        /// <returns>The <see cref="CustomGameMode"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomGameMode Get(GameState gameState) => Get(gameState.GetType());

        /// <summary>
        /// Gets a <see cref="CustomGameMode"/> given the specified <see cref="PlayerState"/>.
        /// </summary>
        /// <param name="playerState">The specified <see cref="PlayerState"/>.</param>
        /// <returns>The <see cref="CustomGameMode"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomGameMode Get(PlayerState playerState) => Get(playerState.GetType());

        /// <summary>
        /// Attempts to retrieve a <see cref="CustomGameMode"/> based on the provided id or <see cref="UUCustomGameModeType"/>.
        /// </summary>
        /// <param name="id">The id or <see cref="UUCustomGameModeType"/> of the custom game mode.</param>
        /// <param name="customGameMode">When this method returns, contains the <see cref="CustomGameMode"/> associated with the specified id, if the id was found; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomGameMode"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object id, out CustomGameMode customGameMode) => customGameMode = Get(id);

        /// <summary>
        /// Tries to get a <see cref="CustomGameMode"/> given the specified <see cref="CustomGameMode"/>.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <param name="customGameMode">The found <see cref="CustomGameMode"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomGameMode"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out CustomGameMode customGameMode) => customGameMode = Get(id);

        /// <summary>
        /// Tries to get a <see cref="CustomGameMode"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomGameMode"/> name to look for.</param>
        /// <param name="customGameMode">The found <see cref="CustomGameMode"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomGameMode"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomGameMode customGameMode) => customGameMode = List.FirstOrDefault(cRole => cRole.Name == name);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomGameMode"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to search for.</param>
        /// <param name="customGameMode">The found <see cref="CustomGameMode"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomGameMode"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomGameMode customGameMode) => customGameMode = Get(type.GetType());

        /// <summary>
        /// Enables all the custom game modes present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomGameMode"/> containing all enabled custom game modes.</returns>
        public static List<CustomGameMode> EnableAll() => EnableAll(Assembly.GetCallingAssembly());

        /// <summary>
        /// Enables all the custom game modes present in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to enable the game modes from.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomGameMode"/> containing all enabled custom game modes.</returns>
        public static List<CustomGameMode> EnableAll(Assembly assembly)
        {
            List<CustomGameMode> customGameModes = new();
            foreach (Type type in assembly.GetTypes())
            {
                CustomGameModeAttribute attribute = type.GetCustomAttribute<CustomGameModeAttribute>();
                if (!typeof(CustomGameMode).IsAssignableFrom(type) || attribute is null)
                    continue;

                CustomGameMode customGameMode = Activator.CreateInstance(type) as CustomGameMode;

                if (!customGameMode.IsEnabled)
                    continue;

                if (customGameMode.BehaviourComponents.Count(comp => typeof(GameState).IsAssignableFrom(comp)) != 1 || customGameMode.PlayerStates.Count() <= 0)
                {
                    Log.Error($"Failed to load the custom game mode.\n" +
                              $"The game mode \"{customGameMode.Name}\" should have exactly one GameState component and at least one PlayerState component defined.");
                    continue;
                }

                if (customGameMode.TryRegister(attribute))
                    customGameModes.Add(customGameMode);
            }

            if (customGameModes.Count() != Registered.Count)
                Log.Info($"{customGameModes.Count()} custom game modes have been successfully registered!");

            return customGameModes;
        }

        /// <summary>
        /// Disables all the custom game modes present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomGameMode"/> containing disabled custom game modes.</returns>
        public static List<CustomGameMode> DisableAll()
        {
            List<CustomGameMode> customGameModes = new();
            customGameModes.AddRange(List.Where(customGameMode => customGameMode.TryUnregister()));

            Log.Info($"{customGameModes.Count()} custom game modes have been successfully unregistered!");

            return customGameModes;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomGameMode"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomGameModeAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomGameMode"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomGameModeAttribute attribute = null)
        {
            if (!Registered.Contains(this))
            {
                if (attribute is not null && Id == 0)
                {
                    if (attribute.Id != 0)
                        Id = attribute.Id;
                    else
                        throw new ArgumentException($"Unable to register {Name}. The ID 0 is reserved for special use.");
                }

                CustomGameMode duplicate = Registered.FirstOrDefault(x => x.Id == Id || x.Name == Name || x.BehaviourComponents.Any(comp => BehaviourComponents.Contains(comp)));
                if (duplicate)
                {
                    Log.Debug($"Unable to register {Name}. Another game mode has been registered with the same ID, Name or Behaviour Component: {duplicate.Name}");

                    return false;
                }

                foreach (Type t in BehaviourComponents)
                    EObject.RegisterObjectType(t, typeof(GameState).IsAssignableFrom(t) ? $"GameState-{Name}" : typeof(PlayerState).IsAssignableFrom(t) ? $"PlayerState-{Name}" : Name);

                Registered.Add(this);

                TypeLookupTable.TryAdd(GetType(), this);
                BehavioursLookupTable.TryAdd(BehaviourComponents, this);
                IdLookupTable.TryAdd(Id, this);
                NameLookupTable.TryAdd(Name, this);

                return true;
            }

            Log.Debug($"Unable to register {Name}. Another identical game mode already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomGameMode"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomGameMode"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug($"Unable to unregister {Name}. Game mode is not yet registered.");

                return false;
            }

            BehaviourComponents.ForEach(comp => EObject.UnregisterObjectType(comp));
            Registered.Remove(this);

            TypeLookupTable.Remove(GetType());
            BehavioursLookupTable.Remove(BehaviourComponents);
            IdLookupTable.Remove(Id);
            NameLookupTable.Remove(Name);

            return true;
        }
    }
}