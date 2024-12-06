// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Features;
    using Properties;
    using Query;
    using RemoteAdmin;
    using YamlDotNet.Core;
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
        public static Dictionary<string, Group> Groups { get; internal set; } = new();

        /// <summary>
        /// Gets the default group.
        /// </summary>
        public static Group DefaultGroup
        {
            get
            {
                foreach (KeyValuePair<string, Group> group in Groups)
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
            if (ServerStatic.PermissionsHandler is null)
            {
                Log.Error("Your Remote Admin config is broken. You have to fix it because the game won't even start with a broken config.");

                // If we don't return the context, it'll throw another exception.
                return;
            }

            try
            {
                Dictionary<string, object> rawDeserializedPerms = Deserializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Instance.Config.FullPath)) ?? DictionaryPool<string, object>.Pool.Get();
                Dictionary<string, Group> deserializedPerms = new();
                foreach (KeyValuePair<string, object> group in rawDeserializedPerms)
                {
                    try
                    {
                        if (string.Equals(group.Key, "user", StringComparison.OrdinalIgnoreCase) || ServerStatic.PermissionsHandler._groups.ContainsKey(group.Key))
                        {
                            deserializedPerms.Add(group.Key, Deserializer.Deserialize<Group>(Serializer.Serialize(group.Value)));
                        }
                        else
                        {
                            Log.Warn($"{group.Key} is not a valid permission group.");
                        }
                    }
                    catch (YamlException exception)
                    {
                        Log.Error($"Unable to parse permission config for: {group.Key}.\n{exception.Message}.\nEnable debug to see stacktrace.");
                        Log.Debug($"{exception.Message}\n{exception.StackTrace}");
                    }
                }

                DictionaryPool<string, object>.Pool.Return(rawDeserializedPerms);

                Groups = deserializedPerms;
            }
            catch (Exception e)
            {
                Log.Error($"Unable to parse permission config:\n{e}.\nMake sure your config file is setup correctly, every group defined must include inheritance and permissions values, even if they are empty.");
            }

            foreach (KeyValuePair<string, Group> group in Groups.Reverse())
            {
                try
                {
                    IEnumerable<string> inheritedPerms = new List<string>();

                    inheritedPerms = Groups.Where(pair => group.Value.Inheritance.Contains(pair.Key))
                        .Aggregate(inheritedPerms, (current, pair) => current.Union(pair.Value.CombinedPermissions));

                    group.Value.CombinedPermissions = group.Value.Permissions.Union(inheritedPerms).ToList();

                    IEnumerable<string> inheritedDisabledPerms = new List<string>();

                    inheritedDisabledPerms = Groups.Where(pair => group.Value.Inheritance.Contains(pair.Key))
                        .Aggregate(inheritedDisabledPerms, (current, pair) => current.Union(pair.Value.CombinedDisabledPermissions));

                    group.Value.CombinedDisabledPermissions = group.Value.DisabledPermissions.Union(inheritedDisabledPerms).ToList();

                    Log.Debug($"{group.Key} permissions loaded.");
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load permissions/inheritance for: {group.Key}.\n{e.Message}.\nMake sure your config file is setup correctly, every group defined must include inheritance and permissions values, even if they are empty.");
                    Log.Debug($"{e}");
                }
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
            if (sender.FullPermissions || sender is ServerConsoleSender || sender == Server.Host.Sender)
            {
                return true;
            }
            else if (sender is PlayerCommandSender || sender is QueryCommandSender)
            {
                return Player.Get(sender.SenderId) is Player player && player.CheckPermission(permission);
            }

            return false;
        }

        /// <summary>
        /// Checks a player's permission.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <returns><see langword="true"/> if the player's current or native group has permissions; otherwise, <see langword="false"/>.</returns>
        public static bool CheckPermission(this Player player, string permission)
        {
            if (string.IsNullOrEmpty(permission))
                return false;

            if (Server.Host == player)
                return true;

            if (player is null || player.GameObject == null || Groups is null || Groups.Count == 0)
                return false;

            Log.Debug($"UserID: {player.UserId} | PlayerId: {player.Id}");
            Log.Debug($"Permission string: {permission}");

            string plyGroupKey = player.Group is not null ? ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value.EqualsTo(player.Group)).Key : null;
            Log.Debug($"GroupKey: {plyGroupKey ?? "(null)"}");

            if (plyGroupKey is null || !Groups.TryGetValue(plyGroupKey, out Group group))
            {
                Log.Debug("The source group is null, the default group is used");
                group = DefaultGroup;
            }

            if (group is null)
            {
                Log.Debug("There's no default group, returning false...");
                return false;
            }

            const char permSeparator = '.';
            const string allPerms = ".*";

            if (group.CombinedDisabledPermissions.Contains(permission, StringComparison.OrdinalIgnoreCase))
                return false;

            if (permission.Contains(permSeparator))
            {
                StringBuilder strBuilder = StringBuilderPool.Pool.Get();
                string[] seraratedPermissions = permission.Split(permSeparator);

                bool Check(string source) => group.CombinedPermissions.Contains(source, StringComparison.OrdinalIgnoreCase);
                bool CheckDisabled(string source) => group.CombinedDisabledPermissions.Contains(source, StringComparison.OrdinalIgnoreCase);

                bool result = group.CombinedPermissions.Contains(allPerms);

                if (!result)
                {
                    for (int z = 0; z < seraratedPermissions.Length; z++)
                    {
                        if (z != 0)
                        {
                            // We need to clear the last ALL_PERMS line
                            // or it'll be like 'permission.*.subpermission'.
                            strBuilder.Length -= allPerms.Length;

                            // Separate permission groups by using its separator.
                            strBuilder.Append(permSeparator);
                        }

                        strBuilder.Append(seraratedPermissions[z]);

                        // If it's the last index,
                        // then we don't need to check for all permissions of the subpermission.
                        if (z == seraratedPermissions.Length - 1)
                        {
                            result = Check(strBuilder.ToString());
                            break;
                        }

                        strBuilder.Append(allPerms);
                        if (Check(strBuilder.ToString()))
                        {
                            result = true;
                            break;
                        }
                    }

                    if (!result)
                        return false;
                }

                for (int z = 0; z < seraratedPermissions.Length; z++)
                {
                    if (z != 0)
                    {
                        // We need to clear the last ALL_PERMS line
                        // or it'll be like 'permission.*.subpermission'.
                        strBuilder.Length -= allPerms.Length;

                        // Separate permission groups by using its separator.
                        strBuilder.Append(permSeparator);
                    }

                    strBuilder.Append(seraratedPermissions[z]);

                    // If it's the last index,
                    // then we don't need to check for all permissions of the subpermission.
                    if (z == seraratedPermissions.Length - 1)
                    {
                        result = !CheckDisabled(strBuilder.ToString());
                        break;
                    }

                    strBuilder.Append(allPerms);
                    if (CheckDisabled(strBuilder.ToString()))
                    {
                        result = false;
                        break;
                    }
                }

                StringBuilderPool.Pool.Return(strBuilder);

                Log.Debug($"Result in the block: {result}");
                return result;
            }

            // It'll work when there is no dot in the permission.
            bool result2 = group.CombinedPermissions.Contains(permission, StringComparison.OrdinalIgnoreCase) || group.CombinedPermissions.Contains(allPerms);

            if (group.CombinedDisabledPermissions.Contains(permission, StringComparison.OrdinalIgnoreCase))
                result2 = false;

            Log.Debug($"Result outside the block: {result2}");

            return result2;
        }

        /// <summary>
        /// Checks a sender's permission.
        /// </summary>
        /// <param name="sender">The sender to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <param name="response">A response. Contains text only if function returned <see langword="false"/>, otherwise will be <see langword="null"/>.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this ICommandSender sender, string permission, out string response)
        {
            if (sender.CheckPermission(permission))
            {
                response = null;
                return true;
            }

            response = $"You don't have permission to execute this command. Required permission: {permission}";
            return false;
        }

        /// <summary>
        /// Checks a sender's permission.
        /// </summary>
        /// <param name="sender">The sender to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <param name="response">A response. Contains text only if function returned <see langword="false"/>, otherwise will be <see langword="null"/>.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this CommandSender sender, string permission, out string response)
            => (sender as ICommandSender).CheckPermission(permission, out response);

        /// <summary>
        /// Checks a sender's permission.
        /// </summary>
        /// <param name="sender">The sender to be checked.</param>
        /// <param name="permission">The permission to be checked.</param>
        /// <param name="response">A response. Contains text only if function returned <see langword="false"/>, otherwise will be <see langword="null"/>.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this Player sender, string permission, out string response)
            => sender.Sender.CheckPermission(permission, out response);

        /// <summary>
        /// Checks a player's permission.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <param name="permissions">The permission for checking.</param>
        /// <returns>Returns a value indicating whether the user has the permission or not.</returns>
        public static bool CheckPermission(this Player player, params PlayerPermissions[] permissions)
            => permissions.All(permission => CommandProcessor.CheckPermissions(player.Sender, permission));
    }
}