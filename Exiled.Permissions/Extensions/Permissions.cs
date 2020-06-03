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
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using static Exiled.Permissions.Config;

    /// <inheritdoc cref="Exiled.Permissions.Permissions"/>
    public static class Permissions
    {
        private static readonly IDeserializer Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        private static readonly ISerializer Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

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
            if (!Directory.Exists(Folder))
            {
                Log.Warn("Permissions directory missing, creating.");
                Directory.CreateDirectory(Folder);
            }

            if (!File.Exists(FullPath))
            {
                Log.Warn("Permissions file missing, creating.");
                File.WriteAllText(FullPath, Encoding.UTF8.GetString(Resources.permissions));
            }
        }

        /// <summary>
        /// Reloads permissions.
        /// </summary>
        public static void Reload() => Groups = Deserializer.Deserialize<Dictionary<string, Group>>(File.ReadAllText(FullPath));

        /// <summary>
        /// Save permissions.
        /// </summary>
        public static void Save() => File.WriteAllText(FullPath, Serializer.Serialize(Groups));

        /// <summary>
        /// Checks a player's permission.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this Player player, string permission)
        {
            if (player.GameObject == PlayerManager.localPlayer)
            {
                return true;
            }
            else if (player == null)
            {
                Log.Error("Reference hub was null, unable to check permissions.");
                return false;
            }

            Log.Debug($"Player: {player.Nickname} UserID: {player.UserId}", PluginManager.ShouldDebugBeShown);
            if (string.IsNullOrEmpty(permission))
            {
                Log.Error("Permission checked was null.");
                return false;
            }

            Log.Debug($"Permission string: {permission}", PluginManager.ShouldDebugBeShown);
            UserGroup userGroup = ServerStatic.GetPermissionsHandler().GetUserGroup(player.UserId);
            Group group = null;

            if (userGroup != null)
            {
                Log.Debug($"UserGroup: {userGroup.BadgeText}", PluginManager.ShouldDebugBeShown);
                string groupName = ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value == player.Group).Key;
                Log.Debug($"GroupName: {groupName}", PluginManager.ShouldDebugBeShown);
                if (Groups == null)
                {
                    Log.Error("Permissions config is null.");
                    return false;
                }

                if (!Groups.Any())
                {
                    Log.Error("No permissionconfig groups.");
                    return false;
                }

                if (!Groups.TryGetValue(groupName, out group))
                {
                    Log.Error("Could not get permission value.");
                    return false;
                }

                Log.Debug($"Got group.", PluginManager.ShouldDebugBeShown);
            }
            else
            {
                Log.Debug("Player group is null, getting default..", PluginManager.ShouldDebugBeShown);
                group = DefaultGroup;
            }

            if (group != null)
            {
                Log.Debug("Group is not null!", PluginManager.ShouldDebugBeShown);
                if (permission.Contains("."))
                {
                    Log.Debug("Group contains permission separator", PluginManager.ShouldDebugBeShown);
                    if (group.Permissions.Any(s => s == ".*"))
                    {
                        Log.Debug("All permissions have been granted for all nodes.", PluginManager.ShouldDebugBeShown);
                        return true;
                    }

                    if (group.Permissions.Contains(permission.Split('.')[0] + ".*"))
                    {
                        Log.Debug("Check 1: True, returning.", PluginManager.ShouldDebugBeShown);
                        return true;
                    }
                }

                if (group.Permissions.Contains(permission) || group.Permissions.Contains("*"))
                {
                    Log.Debug("Check 2: True, returning.", PluginManager.ShouldDebugBeShown);
                    return true;
                }
            }
            else
            {
                Log.Debug("Group is null, returning false.", PluginManager.ShouldDebugBeShown);
                return false;
            }

            Log.Debug("No permissions found.", PluginManager.ShouldDebugBeShown);

            return false;
        }
    }
}
