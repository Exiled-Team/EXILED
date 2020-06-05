// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    /// <summary>
    /// A set of extensions for <see cref="RoleType"/>.
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// Get the <see cref="Team"/> of the given <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">Role.</param>
        /// <returns><see cref="Team"/>.</returns>
        public static Team GetTeam(this RoleType roleType)
        {
            switch (roleType)
            {
                case RoleType.ChaosInsurgency:
                    return Team.CHI;
                case RoleType.Scientist:
                    return Team.RSC;
                case RoleType.ClassD:
                    return Team.CDP;
                case RoleType.Scp049:
                case RoleType.Scp93953:
                case RoleType.Scp93989:
                case RoleType.Scp0492:
                case RoleType.Scp079:
                case RoleType.Scp096:
                case RoleType.Scp106:
                case RoleType.Scp173:
                    return Team.SCP;
                case RoleType.Spectator:
                    return Team.RIP;
                case RoleType.FacilityGuard:
                case RoleType.NtfCadet:
                case RoleType.NtfLieutenant:
                case RoleType.NtfCommander:
                case RoleType.NtfScientist:
                    return Team.MTF;
                case RoleType.Tutorial:
                    return Team.TUT;
                default:
                    return Team.RIP;
            }
        }
    }
}