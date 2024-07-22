// -----------------------------------------------------------------------
// <copyright file="SpawnReason.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Core.Generic;
    using PlayerRoles;

    /// <summary>
    /// Possible spawn reasons.
    /// </summary>
    public sealed class SpawnReason : EnumClass<RoleChangeReason, SpawnReason>
    {
        /// <summary>
        /// No reason specified.
        /// </summary>
        public static readonly SpawnReason None = new(RoleChangeReason.None);

        /// <summary>
        /// The round has just started.
        /// </summary>
        public static readonly SpawnReason RoundStart = new(RoleChangeReason.RoundStart);

        /// <summary>
        /// The player joined and the round recently started.
        /// </summary>
        public static readonly SpawnReason LateJoin = new(RoleChangeReason.LateJoin);

        /// <summary>
        /// The player was dead and is respawning.
        /// </summary>
        public static readonly SpawnReason Respawn = new(RoleChangeReason.Respawn);

        /// <summary>
        /// The player has died.
        /// </summary>
        public static readonly SpawnReason Died = new(RoleChangeReason.Died);

        /// <summary>
        /// The player has escaped.
        /// </summary>
        public static readonly SpawnReason Escaped = new(RoleChangeReason.Escaped);

        /// <summary>
        /// The player was revived by SCP-049.
        /// </summary>
        public static readonly SpawnReason Revived = new(RoleChangeReason.Revived);

        /// <summary>
        /// The player's role was changed by an admin command.
        /// </summary>
        public static readonly SpawnReason RemoteAdmin = new(RoleChangeReason.RemoteAdmin);

        /// <summary>
        /// The user will be destroyed.
        /// </summary>
        public static readonly SpawnReason Destroyed = new(RoleChangeReason.Destroyed);

        /// <summary>
        /// The player's role was changed by an exiled plugin.
        /// </summary>
        public static readonly SpawnReason ForceClass = new((RoleChangeReason)9);

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnReason"/> class.
        /// Required for YAML deserialization.
        /// </summary>
        private SpawnReason()
            : base()
        {
        }

        private SpawnReason(RoleChangeReason value)
            : base(value)
        {
        }
    }
}