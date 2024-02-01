// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using MEC;
    using PlayerRoles;
    using Respawning;

    /// <summary>
    /// Abstract base class providing a foundation for custom role management, integrating seamlessly with various game behaviors.
    /// </summary>
    /// <remarks>
    /// The <see cref="CustomRole"/> class establishes a robust framework for creating and managing custom roles within the game architecture.
    /// <para>
    /// This class is designed to be utilized in conjunction with the <see cref="IAdditiveBehaviour"/> interface, ensuring easy integration into existing systems for extending and enhancing role-related functionalities.
    /// <br/>Additionally, <see cref="CustomRole"/> implements <see cref="IEquatable{CustomRole}"/> and <see cref="IEquatable{UInt16}"/>, enabling straightforward equality comparisons.
    /// </para>
    /// </remarks>
    public abstract class CustomRole : CustomModule, IAdditiveBehaviour
    {
        private static readonly Dictionary<Pawn, CustomRole> PlayersValue = new();
        private static readonly List<CustomRole> Registered = new();
        private static readonly Dictionary<Type, CustomRole> TypeLookupTable = new();
        private static readonly Dictionary<Type, CustomRole> BehaviourLookupTable = new();
        private static readonly Dictionary<uint, CustomRole> IdLookupTable = new();
        private static readonly Dictionary<string, CustomRole> NameLookupTable = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomRole"/>'s.
        /// </summary>
        public static IEnumerable<CustomRole> List => Registered;

        /// <summary>
        /// Gets all players and their respective <see cref="CustomRole"/>.
        /// </summary>
        public static IReadOnlyDictionary<Pawn, CustomRole> Manager => PlayersValue;

        /// <summary>
        /// Gets all players belonging to a <see cref="CustomRole"/>.
        /// </summary>
        public static IEnumerable<Pawn> Players => PlayersValue.Keys.ToHashSet();

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s <see cref="Type"/>.
        /// </summary>
        public abstract Type BehaviourComponent { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomRole"/>'s id.
        /// </summary>
        public override uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is enabled.
        /// </summary>
        public override bool IsEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether a player can spawn as this <see cref="CustomRole"/> based on its assigned probability.
        /// </summary>
        /// <returns><see langword="true"/> if the probability condition was satisfied; otherwise, <see langword="false"/>.</returns>
        public bool CanSpawnByProbability => UnityEngine.Random.Range(0, 101) <= Probability;

        /// <summary>
        /// Gets all instances of this <see cref="CustomRole"/>.
        /// </summary>
        public int Instances { get; private set; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s description.
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s <see cref="RoleTypeId"/>.
        /// </summary>
        public virtual RoleTypeId Role { get; }

        /// <summary>
        /// Gets the relative spawn chance of the <see cref="CustomRole"/>.
        /// </summary>
        public virtual int Probability { get; }

        /// <summary>
        /// Gets the <see cref="RoleSettings"/>.
        /// </summary>
        public virtual RoleSettings Settings { get; } = RoleSettings.Default;

        /// <summary>
        /// Gets the <see cref="CustomEscapes.EscapeSettings"/>.
        /// </summary>
        public virtual List<EscapeSettings> EscapeSettings { get; } = new();

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s <see cref="Type"/>.
        /// </summary>
        public virtual Type EscapeBehaviourComponent { get; }

        /// <summary>
        /// Gets a value representing the maximum instances of the <see cref="CustomRole"/> that can be automatically assigned.
        /// </summary>
        public virtual int MaxInstances => IsScp ? 1 : -1;

        /// <summary>
        /// Gets the team ownership of the <see cref="CustomRole"/>.
        /// </summary>
        /// <para/>
        /// By setting the ownership, the <see cref="CustomRole"/> will belong to the specified team.
        public virtual Team TeamOwnership { get; }

        /// <summary>
        /// Gets the <see cref="SpawnableTeamType"/> from which to retrieve players for assigning the <see cref="CustomRole"/>.
        /// </summary>
        public virtual SpawnableTeamType AssignFromTeam => SpawnableTeamType.None;

        /// <summary>
        /// Gets the <see cref="RoleTypeId"/> from which to retrieve players for assigning the <see cref="CustomRole"/>.
        /// </summary>
        public virtual RoleTypeId AssignFromRole { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> should be treated as a separate team unit.
        /// </summary>
        public virtual bool IsTeamUnit { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is registered.
        /// </summary>
        public virtual bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> should be considered an SCP.
        /// </summary>
        public bool IsScp => TeamOwnership is Team.SCPs;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pawn"/> containing all players owning this <see cref="CustomRole"/>.
        /// </summary>
        public IEnumerable<Pawn> Owners => Player.Get(x => TryGet(x.Cast<Pawn>(), out CustomRole customRole) && customRole.Id == Id).Cast<Pawn>();

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(uint id) => IdLookupTable[id];

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(string name) => NameLookupTable[name];

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomRole Get(Type type) =>
            typeof(CustomRole).IsAssignableFrom(type) ? TypeLookupTable[type] :
            typeof(RoleBehaviour).IsAssignableFrom(type) ? BehaviourLookupTable[type] : null;

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances belonging to the specified <see cref="Team"/>.
        /// </summary>
        /// <param name="team">The specified <see cref="Team"/>.</param>
        /// <returns>All <see cref="CustomRole"/> instances belonging to the specified <see cref="Team"/>.</returns>
        public static IEnumerable<CustomRole> Get(Team team) => List.Where(customRole => RoleExtensions.GetTeam(customRole.Role) == team);

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances belonging to the specified teams.
        /// </summary>
        /// <param name="teams">The specified teams.</param>
        /// <returns>All <see cref="CustomRole"/> instances belonging to the specified teams.</returns>
        public static IEnumerable<CustomRole> Get(IEnumerable<Team> teams) => List.Where(customRole => teams.Contains(RoleExtensions.GetTeam(customRole.Role)));

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances belonging to the specified teams.
        /// </summary>
        /// <param name="teams">The specified teams.</param>
        /// <returns>All <see cref="CustomRole"/> instances belonging to the specified teams.</returns>
        public static IEnumerable<CustomRole> Get(params Team[] teams) => List.Where(customRole => teams.Contains(RoleExtensions.GetTeam(customRole.Role)));

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="Type"/>.</typeparam>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomRole Get<T>()
            where T : CustomRole => Get(typeof(T));

        /// <summary>
        /// Gets a <see cref="CustomRole"/> from a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="player">The <see cref="CustomRole"/> owner.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(Pawn player) => PlayersValue.TryGetValue(player, out CustomRole customRole) ? customRole : default;

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given the specified id.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out CustomRole customRole) => customRole = Get(id);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomRole"/> name to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomRole customRole) => customRole = Get(name);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to search on.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pawn player, out CustomRole customRole) => customRole = Get(player);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to search for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomRole customRole) => customRole = Get(type);

        /// <summary>
        /// Attempts to spawn the specified player with the provided custom role.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="customRole">The custom role to be assigned to the player.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method attempts to spawn the specified player with the given custom role. If the custom role is not provided
        /// or is invalid, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool TrySpawn(Pawn player, CustomRole customRole)
        {
            if (!customRole)
                return false;

            customRole.Spawn(player);
            return true;
        }

        /// <summary>
        /// Attempts to spawn the specified player with the custom role identified by the provided type or type name.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="id">The id of the custom role to be assigned to the player.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows attempting to spawn the specified player with a custom role identified by its type or type name.
        /// If the custom role type or name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool TrySpawn(Pawn player, uint id)
        {
            if (!TryGet(id, out CustomRole customRole))
                return false;

            TrySpawn(player, customRole);
            return true;
        }

        /// <summary>
        /// Attempts to spawn the specified player with the custom role identified by the provided name.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="name">The name of the custom role to be assigned to the player.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows attempting to spawn the specified player with a custom role identified by its name.
        /// If the custom role name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool TrySpawn(Pawn player, string name)
        {
            if (!TryGet(name, out CustomRole customRole))
                return false;

            TrySpawn(player, customRole);
            return true;
        }

        /// <summary>
        /// Spawns the specified player with the provided custom role.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="customRole">The custom role to be assigned to the player.</param>
        /// <param name="shouldKeepPosition">
        /// A value indicating whether the custom role assignment should maintain the player's current position.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method forces the specified player to respawn with the given custom role. If the custom role is
        /// not provided or is invalid, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Spawn(Pawn player, CustomRole customRole, bool shouldKeepPosition = false)
        {
            if (!customRole)
                return false;

            customRole.ForceSpawn(player, shouldKeepPosition);
            return true;
        }

        /// <summary>
        /// Spawns the specified player with the custom role identified by the provided type or type name.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="id">The id of the custom role to be assigned to the player.</param>
        /// <param name="shouldKeepPosition">
        /// A value indicating whether the custom role assignment should maintain the player's current position.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows the spawning of the specified player with a custom role identified by its type or type name.
        /// If the custom role type or name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Spawn(Pawn player, uint id, bool shouldKeepPosition = false)
        {
            if (!TryGet(id, out CustomRole customRole))
                return false;

            Spawn(player, customRole, shouldKeepPosition);
            return true;
        }

        /// <summary>
        /// Spawns the specified player with the custom role identified by the provided name.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="name">The name of the custom role to be assigned to the player.</param>
        /// <param name="shouldKeepPosition">
        /// A value indicating whether the custom role assignment should maintain the player's current position.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows the spawning of the specified player with a custom role identified by its name.
        /// If the custom role name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Spawn(Pawn player, string name, bool shouldKeepPosition = false)
        {
            if (!TryGet(name, out CustomRole customRole))
                return false;

            Spawn(player, customRole, shouldKeepPosition);
            return true;
        }

        /// <summary>
        /// Removes the custom role from the specified player.
        /// </summary>
        /// <param name="player">The owner of the custom role.</param>
        /// <returns>
        /// <see langword="true"/> if the custom role was removed successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method removes the custom role assigned to the specified player. If the player does not
        /// have a custom role or if the removal operation fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Remove(Pawn player)
        {
            if (!TryGet(player, out CustomRole customRole))
                return false;

            customRole.Eject(player);
            return true;
        }

        /// <summary>
        /// Removes the custom role of type <typeparamref name="T"/> from the specified player.
        /// </summary>
        /// <typeparam name="T">The type of custom role to be removed.</typeparam>
        /// <param name="player">The owner of the custom role.</param>
        /// <returns>
        /// <see langword="true"/> if the custom role was removed successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method removes the custom role of type <typeparamref name="T"/> assigned to the specified player.
        /// If the player does not have the specified custom role or if the removal operation fails,
        /// the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Remove<T>(Pawn player)
            where T : CustomRole => TryGet(typeof(T), out CustomRole customRole) && customRole.Eject(player);

        /// <summary>
        /// Removes the custom role from each player in the specified collection.
        /// </summary>
        /// <param name="players">The collection of players whose custom roles will be removed.</param>
        /// <remarks>
        /// This method removes the custom role from each player in the provided collection. Players without
        /// a custom role or for whom the removal operation fails will be excluded from the removal process.
        /// </remarks>
        public static void Remove(IEnumerable<Pawn> players) => players.ForEach(player => Remove(player));

        /// <summary>
        /// Removes the custom role of type <typeparamref name="T"/> from each player in the specified collection.
        /// </summary>
        /// <typeparam name="T">The type of custom role to be removed.</typeparam>
        /// <param name="players">The collection of players whose custom roles will be removed.</param>
        /// <remarks>
        /// This method removes the custom role of type <typeparamref name="T"/> from each player in the provided collection.
        /// Players without the specified custom role or for whom the removal operation fails will be excluded from the removal process.
        /// </remarks>
        public static void Remove<T>(IEnumerable<Pawn> players)
            where T : CustomRole => players.ForEach(player => Remove<T>(player));

        /// <summary>
        /// Enables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="CustomRole"/> containing all the enabled custom roles.
        /// </returns>
        /// <remarks>
        /// This method dynamically enables all custom roles found in the calling assembly. Custom roles
        /// must be marked with the <see cref="CustomRoleAttribute"/> to be considered for enabling. If
        /// a custom role is enabled successfully, it is added to the returned list.
        /// </remarks>
        public static List<CustomRole> EnableAll()
        {
            List<CustomRole> customRoles = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                CustomRoleAttribute attribute = type.GetCustomAttribute<CustomRoleAttribute>();
                if (!typeof(CustomRole).IsAssignableFrom(type) || attribute is null)
                    continue;

                CustomRole customRole = Activator.CreateInstance(type) as CustomRole;

                if (!customRole.IsEnabled)
                    continue;

                if (customRole.TryRegister(attribute))
                    customRoles.Add(customRole);
            }

            if (customRoles.Count != Registered.Count)
                Log.Info($"{customRoles.Count} custom roles have been successfully registered!");

            return customRoles;
        }

        /// <summary>
        /// Disables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="CustomRole"/> containing all the disabled custom roles.
        /// </returns>
        /// <remarks>
        /// This method dynamically disables all custom roles found in the calling assembly that were
        /// previously registered. If a custom role is disabled successfully, it is added to the returned list.
        /// </remarks>
        public static List<CustomRole> DisableAll()
        {
            List<CustomRole> customRoles = new();
            customRoles.AddRange(Registered.Where(customRole => customRole.TryUnregister()));

            Log.Info($"{customRoles.Count} custom roles have been successfully unregistered!");

            return customRoles;
        }

        /// <summary>
        /// Spawns the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method spawns the specified player as the current  <see cref="CustomRole"/>, adding the
        /// required behavior component. If the player is already alive, the spawn operation
        /// fails and returns <see langword="false"/>.
        /// </remarks>
        public bool Spawn(Pawn player)
        {
            if (player.IsAlive)
                return false;

            player.AddComponent(BehaviourComponent);
            PlayersValue.Remove(player);
            PlayersValue.Add(player, this);
            Instances += 1;

            return true;
        }

        /// <summary>
        /// Spawns each player in the specified collection as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="players">The collection of <see cref="Pawn"/> instances to be spawned.</param>
        /// <remarks>
        /// This method spawns each player in the provided collection as the current  <see cref="CustomRole"/>,
        /// adding the required behavior component. Players that are already alive will not be
        /// affected by the spawn operation. The method is designed for spawning multiple players
        /// with a single call.
        /// </remarks>
        public void Spawn(IEnumerable<Pawn> players) => players.ForEach(player => Spawn(player));

        /// <summary>
        /// Force spawns the specified player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be force spawned.</param>
        /// <remarks>
        /// This method forcefully spawns the player as the current <see cref="CustomRole"/>, regardless of
        /// the player's current state. If the player is not alive, it is immediately spawned;
        /// otherwise, the player's role is temporarily set to Spectator before being force spawned.
        /// </remarks>
        public void ForceSpawn(Pawn player)
        {
            Remove(player);
            PlayersValue.Add(player, this);

            if (!player.IsAlive)
            {
                ForceSpawn_Internal(player, false);
                return;
            }

            player.Role.Set(RoleTypeId.Spectator, SpawnReason.Respawn);
            Timing.CallDelayed(0.1f, () => ForceSpawn_Internal(player, false));
        }

        /// <summary>
        /// Force spawns the specified player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be force spawned.</param>
        /// <param name="preservePosition">A value indicating whether the <see cref="CustomRole"/> assignment should maintain the player's current position.</param>
        /// <remarks>
        /// This method forcefully spawns the player as the current  <see cref="CustomRole"/>, regardless of
        /// the player's current state. If the player is not alive, it is immediately spawned;
        /// otherwise, the player's role is temporarily set to Spectator before being force spawned.
        /// </remarks>
        public void ForceSpawn(Pawn player, bool preservePosition)
        {
            PlayersValue.Remove(player);
            PlayersValue.Add(player, this);

            if (!player.IsAlive)
            {
                ForceSpawn_Internal(player, preservePosition);
                return;
            }

            player.Role.Set(RoleTypeId.Spectator, SpawnReason.Respawn);
            Timing.CallDelayed(0.1f, () => ForceSpawn_Internal(player, preservePosition));
        }

        /// <summary>
        /// Force spawns each player in the specified collection as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="players">The collection of <see cref="Pawn"/> instances to be force spawned.</param>
        /// <remarks>
        /// This method forcefully spawns each player in the provided collection as the current CustomRole,
        /// regardless of their current state. Players that are not alive will be immediately spawned;
        /// otherwise, their roles are temporarily set to Spectator before being force spawned.
        /// </remarks>
        public void ForceSpawn(IEnumerable<Pawn> players) => players.ForEach(player => ForceSpawn(player));

        /// <summary>
        /// Force spawns each player in the specified collection as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="players">The collection of <see cref="Pawn"/> instances to be force spawned.</param>
        /// <param name="preservePosition">A value indicating whether the <see cref="CustomRole"/> assignment should maintain the players' current positions.</param>
        /// <remarks>
        /// This method forcefully spawns each player in the provided collection as the current CustomRole,
        /// regardless of their current state. Players that are not alive will be immediately spawned;
        /// otherwise, their roles are temporarily set to Spectator before being force spawned.
        /// </remarks>
        public void ForceSpawn(IEnumerable<Pawn> players, bool preservePosition) => players.ForEach(player => ForceSpawn(player, preservePosition));

        /// <summary>
        /// Removes the custom role from the specified player.
        /// </summary>
        /// <param name="player">The owner of the custom role.</param>
        /// <returns>
        /// <see langword="true"/> if the custom role was removed successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method removes the custom role assigned to the specified player. If the player does not
        /// have a custom role or if the removal operation fails, the method returns <see langword="false"/>.
        /// </remarks>
        public bool Eject(Pawn player)
        {
            PlayersValue.Remove(player);
            Instances -= 1;

            if (CustomTeam.TryGet(player, out CustomTeam customTeam))
            {
                customTeam.Eject(player);
                return true;
            }

            if (!player.TryGetComponent(BehaviourComponent, out RoleBehaviour rb) || rb.IsDestroying)
                return false;

            rb.Destroy();
            return true;
        }

        /// <summary>
        /// Removes the custom role from each player in the specified collection.
        /// </summary>
        /// <param name="players">The collection of players whose custom roles will be removed.</param>
        /// <remarks>
        /// This method removes the custom role from each player in the provided collection. Players without
        /// a custom role or for whom the removal operation fails will be excluded from the removal process.
        /// </remarks>
        public void Eject(IEnumerable<Pawn> players) => players.ForEach(player => Remove(player));

        /// <summary>
        /// Tries to register a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomRoleAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomRoleAttribute attribute = null)
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

                CustomRole duplicate = Registered.FirstOrDefault(x => x.Id == Id || x.Name == Name || x.BehaviourComponent == BehaviourComponent);
                if (duplicate)
                {
                    Log.Warn($"Unable to register {Name}. Another role with the same ID, Name or Behaviour Component already exists: {duplicate.Name}");

                    return false;
                }

                EObject.RegisterObjectType(BehaviourComponent, Name);
                Registered.Add(this);

                TypeLookupTable.TryAdd(GetType(), this);
                BehaviourLookupTable.TryAdd(BehaviourComponent, this);
                IdLookupTable.TryAdd(Id, this);
                NameLookupTable.TryAdd(Name, this);

                return true;
            }

            Log.Warn($"Unable to register {Name}. Role already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomRole"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Warn($"Unable to unregister {Name}. Role is not yet registered.");

                return false;
            }

            EObject.UnregisterObjectType(BehaviourComponent);
            Registered.Remove(this);

            TypeLookupTable.Remove(GetType());
            BehaviourLookupTable.Remove(BehaviourComponent);
            IdLookupTable.Remove(Id);
            NameLookupTable.Remove(Name);

            return true;
        }

        private void ForceSpawn_Internal(Pawn player, bool preservePosition)
        {
            Instances += 1;
            player.AddComponent(BehaviourComponent, $"ECS-{Name}").Cast<RoleBehaviour>().Settings.PreservePosition = preservePosition;
        }
    }
}
