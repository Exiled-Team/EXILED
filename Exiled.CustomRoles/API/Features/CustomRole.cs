// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="Exiled Team">
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
    using System.Text;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader;
    using InventorySystem;
    using InventorySystem.Configs;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Ammo;
    using MEC;
    using PlayerRoles;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// The custom role base class.
    /// </summary>
    public abstract class CustomRole
    {
        private static Dictionary<Type, CustomRole?> typeLookupTable = new();

        private static Dictionary<string, CustomRole?> stringLookupTable = new();

        private static Dictionary<uint, CustomRole?> idLookupTable = new();

        /// <summary>
        /// Gets a list of all registered custom roles.
        /// </summary>
        public static HashSet<CustomRole> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the custom RoleID of the role.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets or sets the max <see cref="Player.Health"/> for the role.
        /// </summary>
        public abstract int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the name of this role.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this role.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the CustomInfo of this role.
        /// </summary>
        public abstract string CustomInfo { get; set; }

        /// <summary>
        /// Gets all of the players currently set to this role.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> TrackedPlayers { get; } = new();

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> to spawn this role as.
        /// </summary>
        public virtual RoleTypeId Role { get; set; }

        /// <summary>
        /// Gets or sets a list of the roles custom abilities.
        /// </summary>
        public virtual List<CustomAbility>? CustomAbilities { get; set; } = new();

        /// <summary>
        /// Gets or sets the starting inventory for the role.
        /// </summary>
        public virtual List<string> Inventory { get; set; } = new();

        /// <summary>
        /// Gets or sets the starting ammo for the role.
        /// </summary>
        public virtual Dictionary<AmmoType, ushort> Ammo { get; set; } = new();

        /// <summary>
        /// Gets or sets the possible spawn locations for this role.
        /// </summary>
        public virtual SpawnProperties SpawnProperties { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether players keep their current position when gaining this role.
        /// </summary>
        public virtual bool KeepPositionOnSpawn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether players keep their current inventory when gaining this role.
        /// </summary>
        public virtual bool KeepInventoryOnSpawn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether players die when this role is removed.
        /// </summary>
        public virtual bool RemovalKillsPlayer { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether players keep this role when they die.
        /// </summary>
        public virtual bool KeepRoleOnDeath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the Spawn Chance of the Role.
        /// </summary>
        public virtual float SpawnChance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the spawn system is ignored for this role or not.
        /// </summary>
        public virtual bool IgnoreSpawnSystem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether players keep this Custom Role when they switch roles: Class-D -> Scientist for example.
        /// </summary>
        public virtual bool KeepRoleOnChangingRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating broadcast that will be shown to the player.
        /// </summary>
        public virtual Broadcast Broadcast { get; set; } = new Broadcast();

        /// <summary>
        /// Gets or sets a value indicating whether players will receive a message for getting a custom item, when gaining it through the inventory config for this role.
        /// </summary>
        public virtual bool DisplayCustomItemMessages { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating the <see cref="Player"/>'s size.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="string"/> and their  <see cref="Dictionary{TKey, TValue}"/> which is cached Role with FF multiplier.
        /// </summary>
        public virtual Dictionary<RoleTypeId, float> CustomRoleFFMultiplier { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="string"/> for the console message given to players when they receive a role.
        /// </summary>
        public virtual string ConsoleMessage { get; set; } = $"You have spawned as a custom role!";

        /// <summary>
        /// Gets or sets a color of <see cref="ConsoleMessage"/>.
        /// </summary>
        /// <remarks>
        /// You can use <see cref="Color"/> instance in <c>nameof</c>.
        /// </remarks>
        public virtual string ConsoleMessageColor { get; set; } = nameof(Color.green);

        /// <summary>
        /// Gets or sets a <see cref="string"/> for the ability usage help sent to players in the player console.
        /// </summary>
        public virtual string AbilityUsage { get; set; } = "Enter \".special\" in the console to use your ability. If you have multiple abilities, you can use this command to cycle through them, or specify the one to use with \".special ROLENAME AbilityNum\"";

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by ID.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomRole? Get(uint id)
        {
            if (!idLookupTable.ContainsKey(id))
                idLookupTable.Add(id, Registered?.FirstOrDefault(r => r.Id == id));
            return idLookupTable[id];
        }

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by type.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to get.</param>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomRole? Get(Type t)
        {
            if (!typeLookupTable.ContainsKey(t))
                typeLookupTable.Add(t, Registered?.FirstOrDefault(r => r.GetType() == t));
            return typeLookupTable[t];
        }

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomRole? Get(string name)
        {
            if (!stringLookupTable.ContainsKey(name))
                stringLookupTable.Add(name, Registered?.FirstOrDefault(r => r.Name == name));
            return stringLookupTable[name];
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by <inheritdoc cref="Id"/>.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        public static bool TryGet(uint id, out CustomRole? customRole)
        {
            customRole = Get(id);

            return customRole is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(string name, out CustomRole? customRole)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customRole = uint.TryParse(name, out uint id) ? Get(id) : Get(name);

            return customRole is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(Type t, out CustomRole? customRole)
        {
            customRole = Get(t);

            return customRole is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="IReadOnlyCollection{T}"/> of the specified <see cref="Player"/>'s <see cref="CustomRole"/>s.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="customRoles">The custom roles the player has.</param>
        /// <returns>True if the player has custom roles.</returns>
        /// <exception cref="ArgumentNullException">If the player is <see langword="null"/>.</exception>
        public static bool TryGet(Player player, out IReadOnlyCollection<CustomRole> customRoles)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            List<CustomRole> tempList = ListPool<CustomRole>.Pool.Get();
            tempList.AddRange(Registered?.Where(customRole => customRole.Check(player)) ?? Array.Empty<CustomRole>());

            customRoles = tempList.AsReadOnly();
            ListPool<CustomRole>.Pool.Return(tempList);

            return customRoles?.Count > 0;
        }

        /// <summary>
        /// Registers all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all registered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> RegisterRoles(bool skipReflection = false, object? overrideClass = null) => RegisterRoles(skipReflection, overrideClass, true, Assembly.GetCallingAssembly());

        /// <summary>
        /// Registers all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <param name="inheritAttributes">Whether or not inherited attributes should be taken into account for registration.</param>
        /// <param name="assembly">Assembly which is calling this method.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all registered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> RegisterRoles(bool skipReflection = false, object? overrideClass = null, bool inheritAttributes = true, Assembly? assembly = null)
        {
            List<CustomRole> roles = new();

            Log.Warn("Registering roles...");

            assembly ??= Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomRole) && type.GetCustomAttribute(typeof(CustomRoleAttribute), inheritAttributes) is null)
                {
                    Log.Debug($"{type} base: {type.BaseType} -- {type.GetCustomAttribute(typeof(CustomRoleAttribute), inheritAttributes) is null}");
                    continue;
                }

                Log.Debug($"Getting attributed for {type}");
                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), inheritAttributes).Cast<Attribute>())
                {
                    CustomRole? customRole = null;

                    if (!skipReflection && Server.PluginAssemblies.TryGetValue(assembly, out IPlugin<IConfig> plugin))
                    {
                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customRole = property.GetValue(overrideClass ?? plugin.Config) as CustomRole;
                            break;
                        }
                    }

                    customRole ??= (CustomRole)Activator.CreateInstance(type);

                    if (customRole.Role == RoleTypeId.None)
                        customRole.Role = ((CustomRoleAttribute)attribute).RoleTypeId;

                    if (customRole.TryRegister())
                        roles.Add(customRole);
                }
            }

            return roles;
        }

        /// <summary>
        /// Registers all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all registered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> RegisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false, bool skipReflection = false, object? overrideClass = null)
        {
            List<CustomRole> roles = new();
            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomItem) ||
                    type.GetCustomAttribute(typeof(CustomRoleAttribute)) is null ||
                    (isIgnored && targetTypes.Contains(type)) ||
                    (!isIgnored && !targetTypes.Contains(type)))
                {
                    continue;
                }

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), true).Cast<Attribute>())
                {
                    CustomRole? customRole = null;

                    if (!skipReflection && Server.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Server.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ??
                                                          plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customRole = property.GetValue(overrideClass ?? plugin.Config) as CustomRole;
                        }
                    }

                    customRole ??= (CustomRole)Activator.CreateInstance(type);

                    if (customRole.Role == RoleTypeId.None)
                        customRole.Role = ((CustomRoleAttribute)attribute).RoleTypeId;

                    if (customRole.TryRegister())
                        roles.Add(customRole);
                }
            }

            return roles;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all unregistered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> UnregisterRoles()
        {
            List<CustomRole> unregisteredRoles = new();

            foreach (CustomRole customRole in Registered)
            {
                customRole.TryUnregister();
                unregisteredRoles.Add(customRole);
            }

            return unregisteredRoles;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all unregistered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> UnregisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomRole> unregisteredRoles = new();

            foreach (CustomRole customRole in Registered)
            {
                if ((targetTypes.Contains(customRole.GetType()) && isIgnored) || (!targetTypes.Contains(customRole.GetType()) && !isIgnored))
                    continue;

                customRole.TryUnregister();
                unregisteredRoles.Add(customRole);
            }

            return unregisteredRoles;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetRoles">The <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> containing the target roles.</param>
        /// <param name="isIgnored">A value indicating whether the target roles should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all unregistered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> UnregisterRoles(IEnumerable<CustomRole> targetRoles, bool isIgnored = false) => UnregisterRoles(targetRoles.Select(x => x.GetType()), isIgnored);

        /// <summary>
        /// ResyncCustomRole Friendly Fire with Player (Append, or Overwrite).
        /// </summary>
        /// <param name="roleToSync"> <see cref="CustomRole"/> to sync with player. </param>
        /// <param name="player"> <see cref="Player"/> Player to add custom role to. </param>
        /// <param name="overwrite"> <see cref="bool"/> whether to force sync (Overwriting previous information). </param>
        public static void SyncPlayerFriendlyFire(CustomRole roleToSync, Player player, bool overwrite = false)
        {
            if (overwrite)
            {
                player.TryAddCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier, overwrite);
                player.UniqueRole = roleToSync.Name;
            }
            else
            {
                player.TryAddCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier);
            }
        }

        /// <summary>
        /// Force sync CustomRole Friendly Fire with Player (Set to).
        /// </summary>
        /// <param name="roleToSync"> <see cref="CustomRole"/> to sync with player. </param>
        /// <param name="player"> <see cref="Player"/> Player to add custom role to. </param>
        public static void ForceSyncSetPlayerFriendlyFire(CustomRole roleToSync, Player player)
        {
            player.TrySetCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier);
        }

        /// <summary>
        /// Checks if the given player has this role.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>True if the player has this role.</returns>
        public virtual bool Check(Player? player) => player is not null && TrackedPlayers.Contains(player);

        /// <summary>
        /// Initializes this role manager.
        /// </summary>
        public virtual void Init()
        {
            idLookupTable.Add(Id, this);
            typeLookupTable.Add(GetType(), this);
            stringLookupTable.Add(Name, this);
            SubscribeEvents();
        }

        /// <summary>
        /// Destroys this role manager.
        /// </summary>
        public virtual void Destroy()
        {
            idLookupTable.Remove(Id);
            typeLookupTable.Remove(GetType());
            stringLookupTable.Remove(Name);
            UnsubscribeEvents();
        }

        /// <summary>
        /// Handles setup of the role, including spawn location, inventory and registering event handlers and add FF rules.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to add the role to.</param>
        public virtual void AddRole(Player player)
        {
            Log.Debug($"{Name}: Adding role to {player.Nickname}.");
            TrackedPlayers.Add(player);

            if (Role != RoleTypeId.None)
            {
                switch (KeepPositionOnSpawn)
                {
                    case true when KeepInventoryOnSpawn:
                        player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                        break;
                    case true:
                        player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.AssignInventory);
                        break;
                    default:
                        {
                            if (KeepInventoryOnSpawn && player.IsAlive)
                                player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
                            else
                                player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.All);
                            break;
                        }
                }
            }

            Timing.CallDelayed(
                0.25f,
                () =>
                {
                    if (!KeepInventoryOnSpawn)
                    {
                        Log.Debug($"{Name}: Clearing {player.Nickname}'s inventory.");
                        player.ClearInventory();
                    }

                    foreach (string itemName in Inventory)
                    {
                        Log.Debug($"{Name}: Adding {itemName} to inventory.");
                        TryAddItem(player, itemName);
                    }
                });

            Log.Debug($"{Name}: Setting health values.");
            player.Health = MaxHealth;
            player.MaxHealth = MaxHealth;
            player.Scale = Scale;

            Vector3 position = GetSpawnPosition();
            if (position != Vector3.zero)
            {
                player.Position = position;
            }

            Log.Debug($"{Name}: Setting player info");

            player.CustomInfo = $"{player.CustomName}\n{CustomInfo}";
            player.InfoArea &= ~(PlayerInfoArea.Role | PlayerInfoArea.Nickname);

            if (CustomAbilities is not null)
            {
                foreach (CustomAbility ability in CustomAbilities)
                    ability.AddAbility(player);
            }

            ShowMessage(player);
            ShowBroadcast(player);
            RoleAdded(player);
            player.UniqueRole = Name;
            player.TryAddCustomRoleFriendlyFire(Name, CustomRoleFFMultiplier);

            if (!string.IsNullOrEmpty(ConsoleMessage))
            {
                StringBuilder builder = StringBuilderPool.Pool.Get();

                builder.AppendLine(Name);
                builder.AppendLine(Description);
                builder.AppendLine();
                builder.AppendLine(ConsoleMessage);

                if (CustomAbilities?.Count > 0)
                {
                    builder.AppendLine(AbilityUsage);
                    builder.AppendLine("Your custom abilities are:");
                    for (int i = 1; i < CustomAbilities.Count + 1; i++)
                        builder.AppendLine($"{i}. {CustomAbilities[i - 1].Name} - {CustomAbilities[i - 1].Description}");

                    builder.AppendLine(
                        "You can keybind the command for this ability by using \"cmdbind .special KEY\", where KEY is any un-used letter on your keyboard. You can also keybind each specific ability for a role in this way. For ex: \"cmdbind .special g\" or \"cmdbind .special bulldozer 1 g\"");
                }

                player.SendConsoleMessage(StringBuilderPool.Pool.ToStringReturn(builder), ConsoleMessageColor);
            }
        }

        /// <summary>
        /// Removes the role from a specific player and FF rules.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to remove the role from.</param>
        public virtual void RemoveRole(Player player)
        {
            if (!TrackedPlayers.Contains(player))
                return;
            Log.Debug($"{Name}: Removing role from {player.Nickname}");
            TrackedPlayers.Remove(player);
            player.CustomInfo = string.Empty;
            player.InfoArea |= PlayerInfoArea.Role | PlayerInfoArea.Nickname;
            player.Scale = Vector3.one;
            if (CustomAbilities is not null)
            {
                foreach (CustomAbility ability in CustomAbilities)
                {
                    ability.RemoveAbility(player);
                }
            }

            RoleRemoved(player);
            player.UniqueRole = string.Empty;
            player.TryRemoveCustomeRoleFriendlyFire(Name);

            if (RemovalKillsPlayer)
                player.Role.Set(RoleTypeId.Spectator);
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        public void SetFriendlyFire(RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFFMultiplier.ContainsKey(roleToAdd))
            {
                CustomRoleFFMultiplier[roleToAdd] = ffMult;
            }
            else
            {
                CustomRoleFFMultiplier.Add(roleToAdd, ffMult);
            }
        }

        /// <summary>
        /// Wrapper to call <see cref="SetFriendlyFire(RoleTypeId, float)"/>.
        /// </summary>
        /// <param name="roleFF"> Role with FF to add even if it exists. </param>
        public void SetFriendlyFire(KeyValuePair<RoleTypeId, float> roleFF)
        {
            SetFriendlyFire(roleFF.Key, roleFF.Value);
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFFMultiplier.ContainsKey(roleToAdd))
            {
                return false;
            }

            CustomRoleFFMultiplier.Add(roleToAdd, ffMult);
            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="pairedRoleFF"> Role FF multiplier to add. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(KeyValuePair<RoleTypeId, float> pairedRoleFF) => TryAddFriendlyFire(pairedRoleFF.Key, pairedRoleFF.Value);

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="ffRules"> Roles to add with friendly fire values. </param>
        /// <param name="overwrite"> Whether to overwrite current values if they exist. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(Dictionary<RoleTypeId, float> ffRules, bool overwrite = false)
        {
            Dictionary<RoleTypeId, float> temporaryFriendlyFireRules = DictionaryPool<RoleTypeId, float>.Pool.Get();

            foreach (KeyValuePair<RoleTypeId, float> roleFF in ffRules)
            {
                if (overwrite)
                {
                    SetFriendlyFire(roleFF);
                }
                else
                {
                    if (!CustomRoleFFMultiplier.ContainsKey(roleFF.Key))
                    {
                        temporaryFriendlyFireRules.Add(roleFF.Key, roleFF.Value);
                    }
                    else
                    {
                        // Contained Key but overwrite set to false so we do not add any.
                        return false;
                    }
                }
            }

            if (!overwrite)
            {
                foreach (KeyValuePair<RoleTypeId, float> roleFF in temporaryFriendlyFireRules)
                {
                    TryAddFriendlyFire(roleFF);
                }
            }

            DictionaryPool<RoleTypeId, float>.Pool.Return(temporaryFriendlyFireRules);
            return true;
        }

        /// <summary>
        /// Tries to register this role.
        /// </summary>
        /// <returns>True if the role registered properly.</returns>
        internal bool TryRegister()
        {
            if (!CustomRoles.Instance!.Config.IsEnabled)
                return false;

            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same Role ID as another role: {Id}. It will not be registered!");

                    return false;
                }

                Registered.Add(this);
                Init();

                Log.Debug($"{Name} ({Id}) has been successfully registered.");

                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Id}) [{Role}] as it already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister this role.
        /// </summary>
        /// <returns>True if the role is unregistered properly.</returns>
        internal bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name} ({Id}) [{Role}], it hasn't been registered yet.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to add an item to the player's inventory by name.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to try giving the item to.</param>
        /// <param name="itemName">The name of the item to try adding.</param>
        /// <returns>Whether or not the item was able to be added.</returns>
        protected bool TryAddItem(Player player, string itemName)
        {
            if (CustomItem.TryGet(itemName, out CustomItem? customItem))
            {
                customItem?.Give(player, DisplayCustomItemMessages);

                return true;
            }

            if (Enum.TryParse(itemName, out ItemType type))
            {
                if (type.IsAmmo())
                    player.AddAmmo(type.GetAmmoType(), InventoryItemLoader.TryGetItem(type, out ItemBase iBase) && iBase.PickupDropModel is AmmoPickup pickup ? (ushort)pickup.MaxAmmo : (ushort)100);
                else
                    player.AddItem(type);

                return true;
            }

            Log.Warn($"{Name}: {nameof(TryAddItem)}: {itemName} is not a valid ItemType or Custom Item name.");

            return false;
        }

        /// <summary>
        /// Gets a random <see cref="Vector3"/> from <see cref="SpawnProperties"/>.
        /// </summary>
        /// <returns>The chosen spawn location.</returns>
        protected Vector3 GetSpawnPosition()
        {
            if (SpawnProperties is null || SpawnProperties.Count() == 0)
                return Vector3.zero;

            if (SpawnProperties.StaticSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in SpawnProperties.StaticSpawnPoints)
                {
                    double r = Loader.Random.NextDouble() * 100;
                    if (r <= chance)
                        return pos;
                }
            }

            if (SpawnProperties.DynamicSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in SpawnProperties.DynamicSpawnPoints)
                {
                    double r = Loader.Random.NextDouble() * 100;
                    if (r <= chance)
                        return pos;
                }
            }

            if (SpawnProperties.RoleSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in SpawnProperties.RoleSpawnPoints)
                {
                    double r = Loader.Random.NextDouble() * 100;
                    if (r <= chance)
                        return pos;
                }
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Called when the role is initialized to setup internal events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            Log.Debug($"{Name}: Loading events.");
            Exiled.Events.Handlers.Player.ChangingNickname += OnInternalChangingNickname;
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Spawning += OnInternalSpawning;
            Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
        }

        /// <summary>
        /// Called when the role is destroyed to unsubscribe internal event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            foreach (Player player in TrackedPlayers)
                RemoveRole(player);

            Log.Debug($"{Name}: Unloading events.");
            Exiled.Events.Handlers.Player.ChangingNickname -= OnInternalChangingNickname;
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Spawning -= OnInternalSpawning;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
        }

        /// <summary>
        /// Shows the spawn message to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to show the message to.</param>
        protected virtual void ShowMessage(Player player) => player.ShowHint(string.Format(CustomRoles.Instance!.Config.GotRoleHint.Content, Name, Description), CustomRoles.Instance.Config.GotRoleHint.Duration);

        /// <summary>
        /// Shows the spawn broadcast to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to show the message to.</param>
        protected virtual void ShowBroadcast(Player player) => player.Broadcast(Broadcast);

        /// <summary>
        /// Called after the role has been added to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> the role was added to.</param>
        protected virtual void RoleAdded(Player player)
        {
        }

        /// <summary>
        /// Called 1 frame before the role is removed from the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> the role was removed from.</param>
        protected virtual void RoleRemoved(Player player)
        {
        }

        private void OnInternalChangingNickname(ChangingNicknameEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            ev.Player.CustomInfo = $"{ev.NewName}\n{CustomInfo}";
        }

        private void OnInternalSpawning(SpawningEventArgs ev)
        {
            if (!IgnoreSpawnSystem && SpawnChance > 0 && !Check(ev.Player) && ev.Player.Role.Type == Role && Loader.Random.NextDouble() * 100 <= SpawnChance)
                AddRole(ev.Player);
        }

        private void OnInternalChangingRole(ChangingRoleEventArgs ev)
        {
            if (Check(ev.Player) && ((ev.NewRole == RoleTypeId.Spectator && !KeepRoleOnDeath) || (ev.NewRole != RoleTypeId.Spectator && ev.NewRole != Role && !KeepRoleOnChangingRole)))
            {
                RemoveRole(ev.Player);
            }
            else if (Check(ev.Player))
            {
                Log.Debug($"{Name}: Checking ammo stuff {Ammo.Count}");
                if (Ammo.Count > 0)
                {
                    Log.Debug($"{Name}: Clearing ammo");
                    ev.Ammo.Clear();
                    Timing.CallDelayed(
                        0.5f,
                        () =>
                        {
                            foreach (AmmoType type in Enum.GetValues(typeof(AmmoType)))
                            {
                                if (type != AmmoType.None)
                                    ev.Player.SetAmmo(type, Ammo.ContainsKey(type) ? Ammo[type] == ushort.MaxValue ? InventoryLimits.GetAmmoLimit(type.GetItemType(), ev.Player.ReferenceHub) : Ammo[type] : (ushort)0);
                            }
                        });
                }
            }
        }

        private void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (Check(ev.Player))
                ev.Role = Role;
        }

        private void OnDestroying(DestroyingEventArgs ev) => RemoveRole(ev.Player);
    }
}
