// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.Permissions.Features;
    using Exiled.Permissions.Properties;

    using RemoteAdmin;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    using static Exiled.Permissions.Permissions;

    /// <inheritdoc cref="Exiled.Permissions.Permissions"/>
    public static class Permissions
    {
        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .Build();

        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        /// <summary>
        /// Gets groups list.
        /// </summary>
        public static Dictionary<string, Group> Groups { get; internal set; } = new Dictionary<string, Group>();

        /// <summary>
        /// Gets the default group.
        /// </summary>
        public static Group DefaultGroup
        {
            get
            {
                foreach (var group in Groups)
                {
                    if (group.Value.IsDefault)
                        return group.Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Create permissions.
        /// </summary>
        public static void Create()
        {
            if (!Directory.Exists(Instance.Config.Folder))
            {
                Log.Warn($"Permissions directory at {Instance.Config.Folder} is missing, creating.");
                Directory.CreateDirectory(Instance.Config.Folder);
            }

            if (!File.Exists(Instance.Config.FullPath))
            {
                Log.Warn($"Permissions file at {Instance.Config.FullPath} is missing, creating.");
                File.WriteAllText(Instance.Config.FullPath, Encoding.UTF8.GetString(Resources.permissions));
            }
        }

        /// <summary>
        /// Reloads permissions.
        /// </summary>
        public static void Reload()
        {
            Groups = Deserializer.Deserialize<Dictionary<string, Group>>(File.ReadAllText(Instance.Config.FullPath));

            foreach (KeyValuePair<string, Group> group in Groups.Reverse())
            {
                IEnumerable<string> inheritedPerms = new List<string>();

                inheritedPerms = Groups.Where(pair => group.Value.Inheritance.Contains(pair.Key))
                    .Aggregate(inheritedPerms, (current, pair) => current.Union(pair.Value.CombinedPermissions));

                group.Value.CombinedPermissions = group.Value.Permissions.Union(inheritedPerms).ToList();
            }
        }

        /// <summary>
        /// Save permissions.
        /// </summary>
        public static void Save() => File.WriteAllText(Instance.Config.FullPath, Serializer.Serialize(Groups));

        /// <summary>
        /// Checks a sender's permission.
        /// </summary>
        /// <param name="sender">The sender to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this ICommandSender sender, string permission) => CheckPermission(sender as CommandSender, permission);

        /// <summary>
        /// Checks a sender's permission.
        /// </summary>
        /// <param name="sender">The sender to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this CommandSender sender, string permission)
        {
            if (sender.FullPermissions || sender is ServerConsoleSender || sender is GameCore.ConsoleCommandSender)
            {
                return true;
            }
            else if (sender is PlayerCommandSender)
            {
                Player player = Player.Get(sender.SenderId);

                if (player == null)
                    return false;

                return player.CheckPermission(permission);
            }

            return false;
        }

        /// <summary>
        /// Checks a player's permission.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this Player player, string permission)
        {
            if (player == null)
                return false;

            if (player.GameObject == Server.Host.GameObject)
                return true;

            Log.Debug($"Player: {player.Nickname} UserID: {player.UserId}", Instance.Config.ShouldDebugBeShown);

            if (string.IsNullOrEmpty(permission))
            {
                Log.Error("Permission checked was null.");
                return false;
            }

            Log.Debug($"Permission string: {permission}", Instance.Config.ShouldDebugBeShown);

            UserGroup userGroup = ServerStatic.GetPermissionsHandler().GetUserGroup(player.UserId);
            Group group = null;

            if (userGroup != null)
            {
                Log.Debug($"UserGroup: {userGroup.BadgeText}", Instance.Config.ShouldDebugBeShown);

                string groupName = ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value == player.Group).Key;

                Log.Debug($"GroupName: {groupName}", Instance.Config.ShouldDebugBeShown);

                if (Groups == null)
                {
                    Log.Error("Permissions config is null.");
                    return false;
                }

                if (!Groups.Any())
                {
                    Log.Error("No permission config groups.");
                    return false;
                }

                if (!Groups.TryGetValue(groupName, out group))
                {
                    Log.Error($"Could not get \"{groupName}\" permission");
                    return false;
                }

                Log.Debug($"Got group.", Instance.Config.ShouldDebugBeShown);
            }
            else
            {
                Log.Debug("Player group is null, getting default..", Instance.Config.ShouldDebugBeShown);

                group = DefaultGroup;
            }

            if (group != null)
            {
                Log.Debug("Group is not null!", Instance.Config.ShouldDebugBeShown);

                if (permission.Contains("."))
                {
                    Log.Debug("Group contains permission separator", Instance.Config.ShouldDebugBeShown);

                    if (group.CombinedPermissions.Any(s => s == ".*"))
                    {
                        Log.Debug("All permissions have been granted for all nodes.", Instance.Config.ShouldDebugBeShown);
                        return true;
                    }

                    if (group.CombinedPermissions.Contains(permission.Split('.')[0] + ".*"))
                    {
                        Log.Debug("Check 1: True, returning.", Instance.Config.ShouldDebugBeShown);
                        return true;
                    }
                }

                if (group.CombinedPermissions.Contains(permission) || group.CombinedPermissions.Contains("*"))
                {
                    Log.Debug("Check 2: True, returning.", Instance.Config.ShouldDebugBeShown);
                    return true;
                }
            }
            else
            {
                Log.Debug("Group is null, returning false.", Instance.Config.ShouldDebugBeShown);
                return false;
            }

            Log.Debug("No permissions found.", Instance.Config.ShouldDebugBeShown);

            return false;
        }
    }
}
