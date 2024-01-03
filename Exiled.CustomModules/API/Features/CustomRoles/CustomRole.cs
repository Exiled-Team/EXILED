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
    using Exiled.CustomModules.API.Features.CustomEscapes;

    using MEC;

    using PlayerRoles;
    using Respawning;

    /// <summary>
    /// The custom role base class.
    /// </summary>
    public abstract class CustomRole : TypeCastObject<CustomRole>, IAdditiveBehaviour
    {
        /// <inheritdoc cref="Manager"/>
        internal static readonly Dictionary<Pawn, CustomRole> PlayersValue = new();

        private static readonly List<CustomRole> Registered = new();

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
        public static IEnumerable<Pawn> Players => Manager.Keys.ToHashSet();

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s <see cref="Type"/>.
        /// </summary>
        public abstract Type BehaviourComponent { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a value indicating whether a player can spawn as this <see cref="CustomRole"/> based on its assigned probability.
        /// </summary>
        /// <returns><see langword="true"/> if the probability condition was satified; otherwise, <see langword="false"/>.</returns>
        public bool CanSpawnByProbability => UnityEngine.Random.Range(0, 101) <= Chance;

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s description.
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomRole"/>'s id.
        /// </summary>
        public virtual uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s <see cref="RoleTypeId"/>.
        /// </summary>
        public virtual RoleTypeId Role { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> should be considered an SCP.
        /// </summary>
        public virtual bool IsScp { get; }

        /// <summary>
        /// Gets the relative spawn chance of the <see cref="CustomRole"/>.
        /// </summary>
        public virtual int Chance { get; }

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
        public virtual bool IsCustomTeamUnit { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is registered.
        /// </summary>
        public virtual bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pawn"/> containing all players owning this <see cref="CustomRole"/>.
        /// </summary>
        public IEnumerable<Pawn> Owners => Player.Get(x => TryGet(x, out CustomRole customRole) && customRole.Id == Id).Cast<Pawn>();

        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomRole left, object right)
        {
            try
            {
                uint value = (uint)right;
                return left.Id == value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified player has a custom role assigned.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to check for a custom role.</param>
        /// <returns>
        /// <see langword="true"/> if the player has a custom role assigned; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="player"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method checks if the specified player has a custom role assigned using <see cref="CustomRole"/>.
        /// </remarks>
        public static bool HasCustomRole(Pawn player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return TryGet(player, out _);
        }


        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomRole left, object right)
        {
            try
            {
                uint value = (uint)right;
                return left.Id != value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(object left, CustomRole right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomRole right) => right != left;

        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomRole left, CustomRole right) => left.Id == right.Id;

        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomRole left, CustomRole right) => left.Id != right.Id;

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <paramref name="customRoleType"/>.
        /// </summary>
        /// <param name="customRoleType">The specified <see cref="Id"/>.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(object customRoleType) => Registered.FirstOrDefault(customRole => customRole == customRoleType && customRole.IsEnabled);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(string name) => Registered.FirstOrDefault(customRole => customRole.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomRole Get(Type type) => type.BaseType != typeof(RoleBehaviour) ? null : Registered.FirstOrDefault(customRole => customRole.BehaviourComponent == type);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <see cref="RoleBehaviour"/>.
        /// </summary>
        /// <param name="roleBuilder">The specified <see cref="RoleBehaviour"/>.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomRole Get(RoleBehaviour roleBuilder) => Get(roleBuilder.GetType());

        /// <summary>
        /// Gets a <see cref="CustomRole"/> from a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="player">The <see cref="CustomRole"/> owner.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(Pawn player)
        {
            CustomRole customRole = default;

            foreach (KeyValuePair<Pawn, CustomRole> kvp in Manager)
            {
                if (kvp.Key != player)
                    continue;

                customRole = Get(kvp.Value.Id);
            }

            return customRole;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given the specified <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="customRoleType">The <see cref="object"/> to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object customRoleType, out CustomRole customRole) => customRole = Get(customRoleType);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomRole"/> name to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomRole customRole) => customRole = Registered.FirstOrDefault(cRole => cRole.Name == name);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to search on.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pawn player, out CustomRole customRole) => customRole = Get(player);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="roleBuilder">The <see cref="RoleBehaviour"/> to search for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(RoleBehaviour roleBuilder, out CustomRole customRole) => customRole = Get(roleBuilder.GetType());

        /// <summary>
        /// Tries to get the player's current <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to search for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomRole customRole) => customRole = Get(type.GetType());

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
        /// <param name="customRoleType">The type or type name of the custom role to be assigned to the player.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows attempting to spawn the specified player with a custom role identified by its type or type name.
        /// If the custom role type or name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool TrySpawn(Pawn player, object customRoleType)
        {
            if (!TryGet(customRoleType, out CustomRole customRole))
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
        /// <param name="customRoleType">The type or type name of the custom role to be assigned to the player.</param>
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
        public static bool Spawn(Pawn player, object customRoleType, bool shouldKeepPosition = false)
        {
            if (!TryGet(customRoleType, out CustomRole customRole))
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
            where T : CustomRole
        {
            if (!TryGet(typeof(T), out CustomRole customRole) || !player.TryGetComponent(customRole.BehaviourComponent, out _))
                return false;

            customRole.Eject(player);
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
                if ((type.BaseType != typeof(CustomRole) && !type.IsSubclassOf(typeof(CustomRole))) || attribute is null)
                    continue;

                CustomRole customRole = Activator.CreateInstance(type) as CustomRole;

                if (!customRole.IsEnabled)
                    continue;

                if (customRole.TryRegister(attribute))
                    customRoles.Add(customRole);
            }

            if (customRoles.Count != Registered.Count())
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
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is CustomRole customRole && customRole == this;

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();

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
                ForceSpawn_Internal(player);
                return;
            }

            player.Role.Set(RoleTypeId.Spectator, SpawnReason.Respawn);
            Timing.CallDelayed(0.1f, () => ForceSpawn_Internal(player));
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
            if (!TryGet(player, out CustomRole customRole) || !player.TryGetComponent(customRole.BehaviourComponent, out RoleBehaviour rb))
                return false;

            PlayersValue.Remove(player);
            rb.DestroyNextTick = true;
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
                    Id = attribute.Id;

                if (Registered.Any(x => x.Id == Id))
                {
                    Log.Warn(
                        $"Couldn't register {Name}. " +
                        $"Another custom role has been registered with the same id:" +
                        $" {Registered.FirstOrDefault(x => x.Id == Id)}");

                    return false;
                }

                Registered.Add(this);

                return true;
            }

            Log.Warn($"Couldn't register {Name}. This custom role has been already registered.");

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
                Log.Debug($"Couldn't unregister {Name}. This custom role hasn't been registered yet.");

                return false;
            }

            Registered.Remove(this);

            return true;
        }

        private void ForceSpawn_Internal(Pawn player) =>
            player.AddComponent(BehaviourComponent, $"ECS-{Name}");

        private void ForceSpawn_Internal(Pawn player, bool preservePosition) =>
            player.AddComponent(BehaviourComponent, $"ECS-{Name}").Cast<RoleBehaviour>().Settings.PreservePosition = preservePosition;
    }
}
