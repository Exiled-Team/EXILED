// -----------------------------------------------------------------------
// <copyright file="SpectatorRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;

    using Exiled.API.Features.Core.Attributes;
    using PlayerRoles;
    using UnityEngine;

    using SpectatorGameRole = PlayerRoles.Spectating.SpectatorRole;

    /// <summary>
    /// Defines a role that represents a spectator.
    /// </summary>
    public class SpectatorRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpectatorRole"/> class.
        /// </summary>
        /// <param name="baseRole">The encapsulated <see cref="SpectatorGameRole"/>.</param>
        internal SpectatorRole(SpectatorGameRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectatorRole"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/>.</param>
        protected internal SpectatorRole(GameObject gameObject)
            : base(gameObject)
        {
        }

        /// <inheritdoc/>
        [EProperty(readOnly: true, category: nameof(Role))]
        public override RoleTypeId Type => RoleTypeId.Spectator;

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the player died.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public DateTime DeathTime => Round.StartedTime + ActiveTime;

        /// <summary>
        /// Gets the total amount of time the player has been dead.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public TimeSpan DeadTime => DateTime.UtcNow - DeathTime;

        /// <summary>
        /// Gets the <see cref="Player"/>'s death position.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public Vector3 DeathPosition => Base.DeathPosition.Position;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is ready to respawn or not.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public bool IsReadyToRespawn => Base.ReadyToRespawn;

        /// <summary>
        /// Gets currently spectated <see cref="Player"/> by this <see cref="Player"/>. May be <see langword="null"/>.
        /// </summary>
        public Player SpectatedPlayer
        {
            get
            {
                Player spectatedPlayer = Player.Get(Base.SyncedSpectatedNetId);

                return spectatedPlayer != Owner ? spectatedPlayer : null;
            }
        }

        /// <summary>
        /// Gets the game <see cref="SpectatorGameRole"/>.
        /// </summary>
        public new SpectatorGameRole Base { get; }
    }
}
