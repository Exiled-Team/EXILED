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

    using Exiled.API.Features;
    using Exiled.Loader;
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
        public static void Reload() => Groups = Deserializer.Deserialize<Dictionary<string, Group>>(File.ReadAllText(Instance.Config.FullPath));

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

            if (player.GameObject == PlayerManager.localPlayer)
                return true;

            Log.Debug($"Player: {player.Nickname} UserID: {player.UserId}", Loader.ShouldDebugBeShown);

            if (string.IsNullOrEmpty(permission))
            {
                Log.Error("Permission checked was null.");
                return false;
            }

            Log.Debug($"Permission string: {permission}", Loader.ShouldDebugBeShown);

            UserGroup userGroup = ServerStatic.GetPermissionsHandler().GetUserGroup(player.UserId);
            Group group = null;

            if (userGroup != null)
            {
                Log.Debug($"UserGroup: {userGroup.BadgeText}", Loader.ShouldDebugBeShown);

                string groupName = ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value == player.Group).Key;

                Log.Debug($"GroupName: {groupName}", Loader.ShouldDebugBeShown);

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

                Log.Debug($"Got group.", Loader.ShouldDebugBeShown);
            }
            else
            {
                Log.Debug("Player group is null, getting default..", Loader.ShouldDebugBeShown);

                group = DefaultGroup;
            }

            if (group != null)
            {
                Log.Debug("Group is not null!", Loader.ShouldDebugBeShown);

                if (permission.Contains("."))
                {
                    Log.Debug("Group contains permission separator", Loader.ShouldDebugBeShown);

                    if (group.CombinedPermissions.Any(s => s == ".*"))
                    {
                        Log.Debug("All permissions have been granted for all nodes.", Loader.ShouldDebugBeShown);
                        return true;
                    }

                    if (group.CombinedPermissions.Contains(permission.Split('.')[0] + ".*"))
                    {
                        Log.Debug("Check 1: True, returning.", Loader.ShouldDebugBeShown);
                        return true;
                    }
                }

                if (group.CombinedPermissions.Contains(permission) || group.CombinedPermissions.Contains("*"))
                {
                    Log.Debug("Check 2: True, returning.", Loader.ShouldDebugBeShown);
                    return true;
                }
            }
            else
            {
                Log.Debug("Group is null, returning false.", Loader.ShouldDebugBeShown);
                return false;
            }

            Log.Debug("No permissions found.", Loader.ShouldDebugBeShown);

            return false;
        }
    }
}
