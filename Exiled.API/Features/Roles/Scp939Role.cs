// -----------------------------------------------------------------------
// <copyright file="Scp939Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using PlayableScps;

    /// <summary>
    /// Defines a role that represents SCP-939.
    /// </summary>
    public class Scp939Role : Role
    {
        private Scp939 script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp939Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        /// <param name="scp939Type">The type of SCP-939.</param>
        internal Scp939Role(Player player, RoleType scp939Type)
        {
            Owner = player;
            RoleType = scp939Type;
            script = player.CurrentScp as Scp939;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="Scp939"/> script for this role.
        /// </summary>
        public Scp939 Script => script ??= Owner.CurrentScp as Scp939;

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can attack again.
        /// </summary>
        public float AttackCooldown
        {
            get => Script.CurrentBiteCooldown;
            set => Script.CurrentBiteCooldown = value;
        }

        /// <summary>
        /// Gets SCP-939's move speed.
        /// </summary>
        public float MoveSpeed => Script.GetMovementSpeed();

        /// <summary>
        /// Gets a list of players this SCP-939 instance can see regardless of their movement.
        /// </summary>
        public List<Player> VisiblePlayers { get; } = new();

        /// <inheritdoc/>
        internal override RoleType RoleType { get; }
    }
}
