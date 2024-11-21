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
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.Events.EventArgs.CustomRoles;
    using PlayerRoles;
    using Respawning;
    using YamlDotNet.Serialization;

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
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired before a player changes role.
        /// </summary>
        [DynamicEventDispatcher]
        [YamlIgnore]
        public static TDynamicEventDispatcher<ChangingCustomRoleEventArgs> ChangingCustomRoleDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired after a player changes role.
        /// </summary>
        [DynamicEventDispatcher]
        [YamlIgnore]
        public static TDynamicEventDispatcher<ChangedCustomRoleEventArgs> ChangedCustomRoleDispatcher { get; set; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomRole"/>'s.
        /// </summary>
        [YamlIgnore]
        public static IEnumerable<CustomRole> List => Registered;

        /// <summary>
        /// Gets all players and their respective <see cref="CustomRole"/>.
        /// </summary>
        [YamlIgnore]
        public static IReadOnlyDictionary<Pawn, CustomRole> Manager => PlayersValue;

        /// <summary>
        /// Gets all players belonging to a <see cref="CustomRole"/>.
        /// </summary>
        [YamlIgnore]
        public static IEnumerable<Pawn> Players => PlayersValue.Keys.ToHashSet();

        /// <inheritdoc/>
        [YamlIgnore]
        public override ModulePointer Config { get; set; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s <see cref="Type"/>.
        /// </summary>
        [YamlIgnore]
        public abstract Type BehaviourComponent { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomRole"/>'s name.
        /// </summary>
        [Description("The name of the custom role.")]
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CustomRole"/>'s id.
        /// </summary>
        [Description("The id of the custom role.")]
        public override uint Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CustomRole"/>'s description.
        /// </summary>
        [Description("The description of the custom role.")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomRole"/> is enabled.
        /// </summary>
        [Description("Indicates whether the custom role is enabled.")]
        public override bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CustomRole"/>'s <see cref="RoleTypeId"/>.
        /// </summary>
        [Description("The custom role's RoleTypeId.")]
        public virtual RoleTypeId Role { get; set; }

        /// <summary>
        /// Gets or sets the relative spawn chance of the <see cref="CustomRole"/>.
        /// </summary>
        [Description("The custom role's spawn chance.")]
        public virtual int Probability { get; set; }

        /// <summary>
        /// Gets or sets the required <see cref="Team"/> that players must belong to in order to allow the <see cref="CustomRole"/> to spawn.
        /// </summary>
        /// <remarks>
        /// This property specifies the required alive team to be eligible for spawning in the <see cref="CustomRole"/>.
        /// </remarks>
        [Description("The custom role's required team to spawn.")]
        public virtual Team RequiredTeamToSpawn { get; set; } = Team.Dead;

        /// <summary>
        /// Gets or sets the required <see cref="RoleTypeId"/> that players must have to allow the <see cref="CustomRole"/> to spawn.
        /// </summary>
        /// <remarks>
        /// This property specifies the required role type for players to be eligible for spawning in the <see cref="CustomRole"/>.
        /// </remarks>
        [Description("The custom role's required RoleTypeId to spawn.")]
        public virtual RoleTypeId RequiredRoleToSpawn { get; set; } = RoleTypeId.None;

        /// <summary>
        /// Gets or sets the required custom team that players must belong to in order to allow the <see cref="CustomRole"/> to spawn.
        /// </summary>
        /// <remarks>
        /// This property specifies the required alive custom team to be eligible for spawning in the <see cref="CustomRole"/>.
        /// </remarks>
        [Description("The custom role's required custom team to spawn.")]
        public virtual uint RequiredCustomTeamToSpawn { get; set; }

        /// <summary>
        /// Gets or sets the required <see cref="CustomRole"/> that players must have to allow the <see cref="CustomRole"/> to spawn.
        /// </summary>
        /// <remarks>
        /// This property specifies the required custom role for players to be eligible for spawning in the <see cref="CustomRole"/>.
        /// </remarks>
        [Description("The custom role's required custom role to spawn.")]
        public virtual uint RequiredCustomRoleToSpawn { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CustomEscapes.EscapeSettings"/>.
        /// </summary>
        [Description("The escape settings for the custom role.")]
        public virtual List<EscapeSettings> EscapeSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets a value representing the maximum instances of the <see cref="CustomRole"/> that can be automatically assigned.
        /// </summary>
        [Description("The maximum number of instances for the custom role.")]
        public virtual int MaxInstances { get; set; }

        /// <summary>
        /// Gets or sets the required teams for this <see cref="CustomRole"/> to win.
        /// </summary>
        /// <remarks>
        /// This property specifies the teams the <see cref="CustomRole"/> belongs to.
        /// </remarks>
        [Description("The required teams for the custom role to win.")]
        public virtual Team[] TeamsOwnership { get; set; } = { };

        /// <summary>
        /// Gets or sets the <see cref="SpawnableTeamType"/> from which to retrieve players for assigning the <see cref="CustomRole"/>.
        /// </summary>
        [Description("The spawnable team type for assigning players to the custom role.")]
        public virtual SpawnableTeamType AssignFromTeam { get; set; } = SpawnableTeamType.None;

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> from which to retrieve players for assigning the <see cref="CustomRole"/>.
        /// </summary>
        [Description("The role type ID for assigning players to the custom role.")]
        public virtual RoleTypeId AssignFromRole { get; set; }

        /// <summary>
        /// Gets or sets all roles to override, preventing the specified roles from spawning.
        /// </summary>
        [Description("All roles to override and prevent from spawning.")]
        public virtual RoleTypeId[] OverrideScps { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomRole"/> should be treated as a separate team unit.
        /// </summary>
        [Description("Indicates whether the custom role should be treated as a separate team unit.")]
        public virtual bool IsTeamUnit { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleSettings"/>.
        /// </summary>
        [Description("The role settings associated with the custom role.")]
        public virtual RoleSettings Settings { get; set; } = RoleSettings.Default;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomRole"/> should be considered an SCP.
        /// </summary>
        [Description("Indicates whether the custom role should be considered an SCP.")]
        public virtual bool IsScp { get; set; }

        /// <summary>
        /// Gets a value indicating whether a player can spawn as this <see cref="CustomRole"/> based on its assigned probability.
        /// </summary>
        /// <returns><see langword="true"/> if the probability condition was satisfied; otherwise, <see langword="false"/>.</returns>
        [YamlIgnore]
        public bool CanSpawnByProbability => Probability.EvaluateProbability();

        /// <summary>
        /// Gets all instances of this <see cref="CustomRole"/>.
        /// </summary>
        [YamlIgnore]
        public int Instances { get; private set; }

        /// <summary>
        /// Gets all the instances of this <see cref="CustomRole"/> in the global context.
        /// </summary>
        [YamlIgnore]
        public int GlobalInstances { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the role can spawn given a condition.
        /// </summary>
        [YamlIgnore]
        public virtual bool EvaluateConditions
        {
            get
            {
                IEnumerable<Pawn> list = Player.List.Cast<Pawn>();

                if (RequiredTeamToSpawn is not Team.Dead)
                {
                    foreach (Pawn pawn in list)
                    {
                        if ((!pawn.HasCustomRole || !pawn.CustomRole.TeamsOwnership.Contains(RequiredTeamToSpawn)) && pawn.Role.Team != RequiredTeamToSpawn)
                            continue;

                        return true;
                    }
                }

                if (RequiredRoleToSpawn is not RoleTypeId.None)
                {
                    foreach (Pawn pawn in list)
                    {
                        if (pawn.Role == RequiredRoleToSpawn)
                        {
                            if ((RoleExtensions.GetTeam(RequiredRoleToSpawn) is Team.SCPs) != pawn.IsScp)
                                continue;

                            return true;
                        }
                    }
                }

                return (RequiredCustomTeamToSpawn > 0 && CustomTeam.TryGet(RequiredCustomTeamToSpawn, out CustomTeam team) && !team.Owners.IsEmpty()) ||
                       (RequiredCustomRoleToSpawn > 0 && TryGet(RequiredCustomRoleToSpawn, out CustomRole role) && !role.Owners.IsEmpty());
            }
        }

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s <see cref="Type"/>.
        /// </summary>
        [YamlIgnore]
        public virtual Type EscapeBehaviourComponent { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is registered.
        /// </summary>
        [YamlIgnore]
        public virtual bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pawn"/> containing all players owning this <see cref="CustomRole"/>.
        /// </summary>
        [YamlIgnore]
        public IEnumerable<Pawn> Owners => Player.Get(x => TryGet(x.Cast<Pawn>(), out CustomRole customRole) && customRole.Id == Id).Cast<Pawn>();

        /// <summary>
        /// Gets a <see cref="CustomRole"/> based on the provided id or <see cref="UUCustomRoleType"/>.
        /// </summary>
        /// <param name="id">The id or <see cref="UUCustomRoleType"/> of the custom role.</param>
        /// <returns>The <see cref="CustomRole"/> with the specified id, or <see langword="null"/> if no role is found.</returns>
        public static CustomRole Get(object id) => id is uint or UUCustomRoleType ? Get((uint)id) : null;

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(uint id) => IdLookupTable.ContainsKey(id) ? IdLookupTable[id] : null;

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(string name) => NameLookupTable.ContainsKey(name) ? NameLookupTable[name] : null;

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomRole Get(Type type) =>
            typeof(CustomRole).IsAssignableFrom(type) ? TypeLookupTable[type] :
            typeof(RoleBehaviour).IsAssignableFrom(type) ? BehaviourLookupTable[type] : null;

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances based on the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>All <see cref="CustomRole"/> instances matching the predicate.</returns>
        public static IEnumerable<CustomRole> Get(Func<CustomRole, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances belonging to the specified <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="role">The specified <see cref="RoleTypeId"/>.</param>
        /// <returns>All <see cref="CustomRole"/> instances belonging to the specified <see cref="RoleTypeId"/>.</returns>
        public static IEnumerable<CustomRole> Get(RoleTypeId role) => List.Where(customRole => customRole.AssignFromRole == role);

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances belonging to the specified <see cref="Team"/>.
        /// </summary>
        /// <param name="team">The specified <see cref="Team"/>.</param>
        /// <returns>All <see cref="CustomRole"/> instances belonging to the specified <see cref="Team"/>.</returns>
        public static IEnumerable<CustomRole> Get(SpawnableTeamType team) => List.Where(customRole => customRole.AssignFromTeam == team);

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances belonging to the specified <see cref="Team"/>.
        /// </summary>
        /// <param name="team">The specified <see cref="Team"/>.</param>
        /// <returns>All <see cref="CustomRole"/> instances belonging to the specified <see cref="Team"/>.</returns>
        public static IEnumerable<CustomRole> Get(Team team) => List.Where(customRole => RoleExtensions.GetTeam(customRole.Role) == team || customRole.TeamsOwnership.Contains(team));

        /// <summary>
        /// Gets all <see cref="CustomRole"/> instances belonging to the specified teams.
        /// </summary>
        /// <param name="teams">The specified teams.</param>
        /// <returns>All <see cref="CustomRole"/> instances belonging to the specified teams.</returns>
        public static IEnumerable<CustomRole> Get(IEnumerable<Team> teams) => List.Where(customRole =>
            teams.Contains(RoleExtensions.GetTeam(customRole.Role)) || customRole.TeamsOwnership.Any(to => teams.Contains(to)));

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
        /// Attempts to retrieve a <see cref="CustomRole"/> based on the provided id or <see cref="UUCustomRoleType"/>.
        /// </summary>
        /// <param name="id">The id or <see cref="UUCustomRoleType"/> of the custom role.</param>
        /// <param name="customRole">When this method returns, contains the <see cref="CustomRole"/> associated with the specified id, if the id was found; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object id, out CustomRole customRole) => customRole = Get(id);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given the specified id.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out CustomRole customRole)
        {
            customRole = Get(id);
            return customRole is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomRole"/> name to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomRole customRole)
        {
            customRole = Get(name);
            return customRole is not null;
        }

        /// <summary>
        /// Tries to get the player's current <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to search on.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pawn player, out CustomRole customRole)
        {
            customRole = Get(player);
            return customRole is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to search for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomRole customRole)
        {
            customRole = Get(type);
            return customRole is not null;
        }

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
        /// <param name="preservePosition">A value indicating whether the custom role assignment should maintain the player's current position.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason"/> to be set.</param>
        /// <param name="roleSpawnFlags">The <see cref="RoleSpawnFlags"/> to be set.</param>
        /// <param name="force">Indicating whether should override current role.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method forces the specified player to respawn with the given custom role. If the custom role is
        /// not provided or is invalid, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Spawn(Pawn player, CustomRole customRole, bool preservePosition = false, SpawnReason spawnReason = null, RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All, bool force = false)
        {
            if (!customRole)
                return false;

            customRole.Spawn(player, preservePosition, spawnReason, roleSpawnFlags, force);
            return true;
        }

        /// <summary>
        /// Spawns the specified player with the custom role identified by the provided type or type name.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="id">The id of the custom role to be assigned to the player.</param>
        /// <param name="preservePosition">A value indicating whether the custom role assignment should maintain the player's current position.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason"/> to be set.</param>
        /// <param name="roleSpawnFlags">The <see cref="RoleSpawnFlags"/> to be set.</param>
        /// <param name="force">Indicating whether should override the role.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows the spawning of the specified player with a custom role identified by its type or type name.
        /// If the custom role type or name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Spawn(Pawn player, uint id, bool preservePosition = false, SpawnReason spawnReason = null, RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All, bool force = false)
        {
            if (!TryGet(id, out CustomRole customRole))
                return false;

            Spawn(player, customRole, preservePosition, spawnReason, roleSpawnFlags, force);
            return true;
        }

        /// <summary>
        /// Spawns the specified player with the custom role identified by the provided name.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="name">The name of the custom role to be assigned to the player.</param>
        /// <param name="preservePosition">A value indicating whether the custom role assignment should maintain the player's current position.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason"/> to be set.</param>
        /// <param name="roleSpawnFlags">The <see cref="RoleSpawnFlags"/> to be set.</param>
        /// <param name="force">Indicating whether should override the role.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows the spawning of the specified player with a custom role identified by its name.
        /// If the custom role name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Spawn(Pawn player, string name, bool preservePosition = false, SpawnReason spawnReason = null, RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All, bool force = false)
        {
            if (!TryGet(name, out CustomRole customRole))
                return false;

            customRole.Spawn(player, preservePosition, spawnReason, roleSpawnFlags);
            return true;
        }

        /// <summary>
        /// Spawns the specified player with the custom role identified by the provided type.
        /// </summary>
        /// <typeparam name="T">The type of custom role to be added.</typeparam>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned with the custom role successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method add the custom role of type <typeparamref name="T"/> assigned to the specified player.
        /// If the addition operation fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool Spawn<T>(Pawn player)
            where T : CustomRole => TryGet(typeof(T), out CustomRole customRole) && customRole.Spawn(player);

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
        /// <param name="assembly">The assembly to enable the module instances from.</param>
        /// <returns>The amount of enabled module instances.</returns>
        /// <remarks>
        /// This method dynamically enables all module instances found in the calling assembly that were
        /// not previously registered.
        /// </remarks>
        public static int EnableAll(Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            List<CustomRole> customRoles = new();
            foreach (Type type in assembly.GetTypes())
            {
                ModuleIdentifierAttribute attribute = type.GetCustomAttribute<ModuleIdentifierAttribute>();
                if (!typeof(CustomRole).IsAssignableFrom(type) || attribute is null)
                    continue;

                CustomRole customRole = Activator.CreateInstance(type) as CustomRole;
                customRole.DeserializeModule();

                if (!customRole.IsEnabled)
                    continue;

                if (customRole.TryRegister(assembly, attribute))
                    customRoles.Add(customRole);
            }

            return customRoles.Count;
        }

        /// <summary>
        /// Disables all the custom roles present in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to disable the module instances from.</param>
        /// <returns>The amount of disabled module instances.</returns>
        /// <remarks>
        /// This method dynamically disables all module instances found in the calling assembly that were
        /// previously registered.
        /// </remarks>
        public static int DisableAll(Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            List<CustomRole> customRoles = new();
            customRoles.AddRange(Registered.Where(customRole => customRole.GetType().Assembly == assembly && customRole.TryUnregister()));
            return customRoles.Count;
        }

        /// <summary>
        /// Spawns the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to be spawned.</param>
        /// <param name="preservePosition">A value indicating whether the <see cref="CustomRole"/> assignment should maintain the player's current position.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason"/> to be set.</param>
        /// <param name="roleSpawnFlags">The <see cref="RoleSpawnFlags"/> to be set.</param>
        /// <param name="force">indicating whethere the spawn is forced.</param>
        /// <returns>
        /// <see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method spawns the specified player as the current  <see cref="CustomRole"/>, adding the
        /// required behavior component. If the player is already alive, the spawn operation
        /// fails and returns <see langword="false"/>.
        /// </remarks>
        public bool Spawn(Pawn player, bool preservePosition = false, SpawnReason spawnReason = null, RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All, bool force = false)
        {
            if (player is null || (player.IsAlive && !force))
                return false;

            ChangingCustomRoleEventArgs changingCustomRole = new(player, Id);
            ChangingCustomRoleDispatcher.InvokeAll(changingCustomRole);

            if (changingCustomRole.Role is RoleTypeId rId)
            {
                player.SetRole(rId, preservePosition, spawnReason, roleSpawnFlags);
                return true;
            }

            if (!TryGet(changingCustomRole.Role, out CustomRole role))
                return false;

            if (role.Id != Id)
                return role.Spawn(player, preservePosition, spawnReason, roleSpawnFlags);

            object prevRole = player.CustomRole ? player.CustomRole.Id : player.Role.Type;
            player.AddComponent(BehaviourComponent, $"ECS-{Name}");
            PlayersValue.Remove(player);
            PlayersValue.Add(player, this);
            Instances += 1;
            GlobalInstances += 1;

            ChangedCustomRoleEventArgs @event = new(player, prevRole);
            ChangedCustomRoleDispatcher.InvokeAll(@event);

            return true;
        }

        /// <summary>
        /// Spawns each player in the specified collection as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="players">The collection of <see cref="Pawn"/> instances to be spawned.</param>
        /// <param name="force">Indicating whether should override role.</param>
        /// <remarks>
        /// This method spawns each player in the provided collection as the current  <see cref="CustomRole"/>,
        /// adding the required behavior component. Players that are already alive will not be
        /// affected by the spawn operation. The method is designed for spawning multiple players
        /// with a single call.
        /// </remarks>
        public void Spawn(IEnumerable<Pawn> players, bool force = false) => players.ForEach(player => Spawn(player, force: force));

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
            Round.IgnoredPlayers.Remove(player);
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
        /// <param name="assembly">The assembly to try and register from.</param>
        /// <param name="attribute">The specified <see cref="ModuleIdentifierAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was registered; otherwise, <see langword="false"/>.</returns>
        protected override bool TryRegister(Assembly assembly, ModuleIdentifierAttribute attribute = null)
        {
            if (Registered.Contains(this))
            {
                Log.Warn($"Unable to register {Name}. Role already exists.");
                return false;
            }

            if (!typeof(RoleBehaviour).IsAssignableFrom(BehaviourComponent))
            {
                Log.Error($"Unable to register {Name}. Behaviour Component must implement RoleBehaviour.");
                return false;
            }

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

            EObject.RegisterObjectType(BehaviourComponent, Name, assembly);
            Registered.Add(this);

            base.TryRegister(assembly, attribute);

            TypeLookupTable.TryAdd(GetType(), this);
            BehaviourLookupTable.TryAdd(BehaviourComponent, this);
            IdLookupTable.TryAdd(Id, this);
            NameLookupTable.TryAdd(Name, this);

            return true;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomRole"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        protected override bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Warn($"Unable to unregister {Name}. Role is not yet registered.");

                return false;
            }

            EObject.UnregisterObjectType(BehaviourComponent);
            Registered.Remove(this);

            base.TryUnregister();

            TypeLookupTable.Remove(GetType());
            BehaviourLookupTable.Remove(BehaviourComponent);
            IdLookupTable.Remove(Id);
            NameLookupTable.Remove(Name);

            return true;
        }
    }
}
