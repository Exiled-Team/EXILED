// -----------------------------------------------------------------------
// <copyright file="UserGroupExtension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    /// <summary>
    /// Contains a useful extension to compare two <see cref="UserGroup"/>'s.
    /// </summary>
    public static class UserGroupExtension
    {
        /// <summary>
        /// Compares two <see cref="UserGroup"/>'s for equality.
        /// </summary>
        /// <param name="this">The fist <see cref="UserGroup"/>.</param>
        /// <param name="other">The second <see cref="UserGroup"/>.</param>
        /// <returns><c>true</c> if they are equal; otherwise, <c>false</c>.</returns>
        public static bool EqualsTo(this UserGroup @this, UserGroup other)
            => @this.BadgeColor == other.BadgeColor
               && @this.BadgeText == other.BadgeText
               && @this.Permissions == other.Permissions
               && @this.Cover == other.Cover
               && @this.HiddenByDefault == other.HiddenByDefault
               && @this.Shared == other.Shared
               && @this.KickPower == other.KickPower
               && @this.RequiredKickPower == other.RequiredKickPower;
    }
}
