// -----------------------------------------------------------------------
// <copyright file="UserGroupExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Linq;

    using Features;

    /// <summary>
    /// Contains a useful extension to compare two <see cref="UserGroup"/>'s.
    /// </summary>
    public static class UserGroupExtensions
    {
        /// <summary>
        /// Compares two <see cref="UserGroup"/>'s for equality.
        /// </summary>
        /// <param name="this">The first <see cref="UserGroup"/>.</param>
        /// <param name="other">The second <see cref="UserGroup"/>.</param>
        /// <returns><see langword="true"/> if they are equal; otherwise, <see langword="false"/>.</returns>
        public static bool EqualsTo(this UserGroup @this, UserGroup other)
            => (@this.BadgeColor == other.BadgeColor)
               && (@this.BadgeText == other.BadgeText)
               && (@this.Permissions == other.Permissions)
               && (@this.Cover == other.Cover)
               && (@this.HiddenByDefault == other.HiddenByDefault)
               && (@this.Shared == other.Shared)
               && (@this.KickPower == other.KickPower)
               && (@this.RequiredKickPower == other.RequiredKickPower);

        /// <summary>
        /// Searches for a key of a group in the <see cref="PermissionsHandler">RemoteAdmin</see> config.
        /// </summary>
        /// <param name="this">The <see cref="UserGroup"/>.</param>
        /// <returns>The key of that group, or <see langword="null"/> if not found.</returns>
        public static string GetKey(this UserGroup @this) => Server.PermissionsHandler._groups
            .FirstOrDefault(pair => pair.Value.EqualsTo(@this)).Key;

        /// <summary>
        /// Searches for a value of a group in the <see cref="PermissionsHandler">RemoteAdmin</see> config.
        /// </summary>
        /// <param name="groupName">The <see cref="string"/>.</param>
        /// <returns>The value of that group, or <see langword="null"/> if not found.</returns>
        public static UserGroup GetValue(string groupName)
        {
            ServerStatic.GetPermissionsHandler().GetAllGroups().TryGetValue(groupName, out UserGroup userGroup);
            return userGroup;
        }
    }
}