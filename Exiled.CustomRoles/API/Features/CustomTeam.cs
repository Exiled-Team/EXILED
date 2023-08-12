// -----------------------------------------------------------------------
// <copyright file="CustomTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems.API.Features;
    using Respawning;
    using YamlDotNet.Serialization;

    /// <summary>
    /// The custom team base class.
    /// </summary>
    public abstract class CustomTeam
    {
        private static Dictionary<Type, CustomTeam?> typeLookupTable = new();

        private static Dictionary<string, CustomTeam?> stringLookupTable = new();

        private static Dictionary<uint, CustomTeam?> idLookupTable = new();

        /// <summary>
        /// Gets a list of all registered custom team.
        /// </summary>
        public static HashSet<CustomTeam> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the custom RoleID of the team.
        /// </summary>
        public virtual uint Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this team.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount of players respawning per wave.
        /// </summary>
        public abstract uint ResapwnAmount { get; set; }

        /// <summary>
        /// Gets or sets the time before this team sapwn.
        /// This time is noramly use to make CASSIE annonce or animation.
        /// </summary>
        public virtual float TimeBeforeSpawn { get; set; }

        /// <summary>
        /// Gets all roles link to this team.
        /// </summary>
        [YamlIgnore]
        public HashSet<CustomTeam> TrackedTeams { get; } = new();

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> by ID.
        /// </summary>
        /// <param name="id">The ID of the team to get.</param>
        /// <returns>The team, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomTeam? Get(uint id)
        {
            if (!idLookupTable.ContainsKey(id))
                idLookupTable.Add(id, Registered?.FirstOrDefault(r => r.Id == id));
            return idLookupTable[id];
        }

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> by type.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to get.</param>
        /// <returns>The team, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomTeam? Get(Type t)
        {
            if (!typeLookupTable.ContainsKey(t))
                typeLookupTable.Add(t, Registered?.FirstOrDefault(r => r.GetType() == t));
            return typeLookupTable[t];
        }

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> by name.
        /// </summary>
        /// <param name="name">The name of the team to get.</param>
        /// <returns>The team, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomTeam? Get(string name)
        {
            if (!stringLookupTable.ContainsKey(name))
                stringLookupTable.Add(name, Registered?.FirstOrDefault(r => r.Name == name));
            return stringLookupTable[name];
        }

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> by <inheritdoc cref="Id"/>.
        /// </summary>
        /// <param name="id">The ID of the team to get.</param>
        /// <param name="customTeam">The custom team.</param>
        /// <returns>True if the team exists.</returns>
        public static bool TryGet(uint id, out CustomTeam? customTeam)
        {
            customTeam = Get(id);

            return customTeam is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> by name.
        /// </summary>
        /// <param name="name">The name of the team to get.</param>
        /// <param name="customTeam">The custom team.</param>
        /// <returns>True if the team exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(string name, out CustomTeam? customTeam)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customTeam = uint.TryParse(name, out uint id) ? Get(id) : Get(name);

            return customTeam is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> by name.
        /// </summary>
        /// <param name="t">The <see cref="System.Type"/> of the team to get.</param>
        /// <param name="customTeam">The custom team.</param>
        /// <returns>True if the teaù exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(Type t, out CustomTeam? customTeam)
        {
            customTeam = Get(t);

            return customTeam is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="IReadOnlyCollection{T}"/> of the specified <see cref="Player"/>'s <see cref="CustomTeam"/>s.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="customTeam">The custom team the player has.</param>
        /// <returns>True if the player has custom team.</returns>
        /// <exception cref="ArgumentNullException">If the player is <see langword="null"/>.</exception>
        public static bool TryGet(Player player, out IReadOnlyCollection<CustomTeam> customTeam)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            List<CustomTeam> tempList = ListPool<CustomTeam>.Pool.Get();
            tempList.AddRange(Registered?.Where(customTeam => customTeam.Check(player)) ?? Array.Empty<CustomTeam>());

            customTeam = tempList.AsReadOnly();
            ListPool<CustomTeam>.Pool.Return(tempList);

            return customTeam?.Count > 0;
        }

        /// <summary>
        /// Registers all the <see cref="CustomTeam"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all registered <see cref="CustomTeam"/>'s.</returns>
        public static IEnumerable<CustomTeam> RegisterTeam(bool skipReflection = false, object? overrideClass = null) => RegisterTeam(skipReflection, overrideClass, true, Assembly.GetCallingAssembly());

        /// <summary>
        /// Registers all the <see cref="CustomTeam"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <param name="inheritAttributes">Whether or not inherited attributes should be taken into account for registration.</param>
        /// <param name="assembly">Assembly which is calling this method.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all registered <see cref="CustomTeam"/>'s.</returns>
        public static IEnumerable<CustomTeam> RegisterTeam(bool skipReflection = false, object? overrideClass = null, bool inheritAttributes = true, Assembly? assembly = null)
        {
            List<CustomTeam> teams = new();

            Log.Warn("Registering teams...");

            assembly ??= Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomTeam) && type.GetCustomAttribute(typeof(CustomTeamAttribute), inheritAttributes) is null)
                {
                    Log.Debug($"{type} base: {type.BaseType} -- {type.GetCustomAttribute(typeof(CustomTeamAttribute), inheritAttributes) is null}");
                    continue;
                }

                Log.Debug("Getting attributed for {type");
                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomTeamAttribute), inheritAttributes).Cast<Attribute>())
                {
                    CustomTeam? customTeam = null;

                    if (!skipReflection && Server.PluginAssemblies.TryGetValue(assembly, out IPlugin<IConfig> plugin))
                    {
                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customTeam = property.GetValue(overrideClass ?? plugin.Config) as CustomTeam;
                            break;
                        }
                    }

                    customTeam ??= (CustomTeam)Activator.CreateInstance(type);

                    if (customTeam.Id == 0)
                        customTeam.Id = ((CustomTeamAttribute)attribute).TeamId;

                    if (customTeam.TryRegister())
                        teams.Add(customTeam);
                }
            }

            return teams;
        }

        /// <summary>
        /// Registers all the <see cref="CustomTeam"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all registered <see cref="CustomTeam"/>'s.</returns>
        public static IEnumerable<CustomTeam> RegisterTeam(IEnumerable<Type> targetTypes, bool isIgnored = false, bool skipReflection = false, object? overrideClass = null)
        {
            List<CustomTeam> teams = new();
            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomItem) ||
                    type.GetCustomAttribute(typeof(CustomTeamAttribute)) is null ||
                    (isIgnored && targetTypes.Contains(type)) ||
                    (!isIgnored && !targetTypes.Contains(type)))
                {
                    continue;
                }

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomTeamAttribute), true).Cast<Attribute>())
                {
                    CustomTeam? customTeam = null;

                    if (!skipReflection && Server.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Server.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ??
                                                          plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customTeam = property.GetValue(overrideClass ?? plugin.Config) as CustomTeam;
                        }
                    }

                    customTeam ??= (CustomTeam)Activator.CreateInstance(type);

                    if (customTeam.Id == 0)
                        customTeam.Id = ((CustomTeamAttribute)attribute).TeamId;

                    if (customTeam.TryRegister())
                        teams.Add(customTeam);
                }
            }

            return teams;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomTeam"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all unregistered <see cref="CustomTeam"/>'s.</returns>
        public static IEnumerable<CustomTeam> UnregisterTeams()
        {
            List<CustomTeam> unregisteredTeams = new();

            foreach (CustomTeam customTeam in Registered)
            {
                customTeam.TryUnregister();
                unregisteredTeams.Add(customTeam);
            }

            return unregisteredTeams;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomTeam"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all unregistered <see cref="CustomTeam"/>'s.</returns>
        public static IEnumerable<CustomTeam> UnregisterTeams(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomTeam> unregisteredTeams = new();

            foreach (CustomTeam customTeam in Registered)
            {
                if ((targetTypes.Contains(customTeam.GetType()) && isIgnored) || (!targetTypes.Contains(customTeam.GetType()) && !isIgnored))
                    continue;

                customTeam.TryUnregister();
                unregisteredTeams.Add(customTeam);
            }

            return unregisteredTeams;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomTeam"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTeams">The <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> containing the target teams.</param>
        /// <param name="isIgnored">A value indicating whether the target teams should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all unregistered <see cref="CustomTeam"/>'s.</returns>
        public static IEnumerable<CustomTeam> UnregisterTeams(IEnumerable<CustomTeam> targetTeams, bool isIgnored = false) => UnregisterTeams(targetTeams.Select(x => x.GetType()), isIgnored);

        /// <summary>
        /// Calculates if the team should spawn or not.
        /// </summary>
        /// <param name="spawnableTeam">The team that will spawn if the return value is <see langword="false"/>.</param>
        /// <returns><see langword="true"/>If the team can spawn.</returns>
        public abstract bool ReadyToSpawn(SpawnableTeamType spawnableTeam);

        /// <summary>
        /// Spawn the team.
        /// </summary>
        /// <param name="players">The spawning players.</param>
        public abstract void Spawn(List<Player> players);

        /// <summary>
        /// Checks if the given player his is Team.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>True if the player has this team.</returns>
        public virtual bool Check(Player? player) => player is not null && TrackedTeams.Any(p => p.Check(player));

        /// <summary>
        /// Initializes this team manager.
        /// </summary>
        public virtual void Init()
        {
            idLookupTable.Add(Id, this);
            typeLookupTable.Add(GetType(), this);
            stringLookupTable.Add(Name, this);
            SubscribeEvents();
        }

        /// <summary>
        /// Destroys this team manager.
        /// </summary>
        public virtual void Destroy()
        {
            idLookupTable.Remove(Id);
            typeLookupTable.Remove(GetType());
            stringLookupTable.Remove(Name);
            UnsubscribeEvents();
        }

        /// <summary>
        /// Tries to register this team.
        /// </summary>
        /// <returns>True if the team registered properly.</returns>
        internal bool TryRegister()
        {
            if (!CustomRole.Instance!.Config.IsEnabled)
                return false;

            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same Team ID as another team: {Id}. It will not be registered!");

                    return false;
                }

                Registered.Add(this);
                Init();

                Log.Debug($"{Name} ({Id}) has been successfully registered.");

                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Id}) as it already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister this team.
        /// </summary>
        /// <returns>True if the team is unregistered properly.</returns>
        internal bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name} ({Id}), it hasn't been registered yet.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Called when the team is initialized to setup internal events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            Log.Debug($"{Name}: Loading events.");
            Exiled.Events.Handlers.Server.SelectTeam += OnSelectTeam;
            Exiled.Events.Handlers.Server.RespawningCustomTeam += OnRespawningCustomTeam; ;
        }

        /// <summary>
        /// Called when the team is destroyed to unsubscribe internal event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            Log.Debug($"{Name}: Unloading events.");
            Exiled.Events.Handlers.Server.SelectTeam -= OnSelectTeam;
            Exiled.Events.Handlers.Server.RespawningCustomTeam -= OnRespawningCustomTeam;
        }

        private void OnSelectTeam(Exiled.Events.EventArgs.Server.SelectTeamEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            if (ReadyToSpawn(ev.SelectedTeam))
            {
                ev.TimeBeforeSpawning = TimeBeforeSpawn;
                ev.SelectedTeam = Respawning.SpawnableTeamType.None;
            }
        }

        private void OnRespawningCustomTeam(Exiled.Events.EventArgs.Server.RespawningCustomTeamEventArgs ev)
        {
            Spawn(ev.Players);
        }
    }
}