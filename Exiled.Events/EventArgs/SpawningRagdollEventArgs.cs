// -----------------------------------------------------------------------
// <copyright file="SpawningRagdollEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using PlayerStatsSystem;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before spawning a player ragdoll.
    /// </summary>
    public class SpawningRagdollEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningRagdollEventArgs"/> class.
        /// </summary>
        /// <param name="ragdollInfo"><inheritdoc cref="RagdollInfo"/></param>
        /// <param name="damageHandlerBase"><inheritdoc cref="DamageHandlerBase"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningRagdollEventArgs(
            RagdollInfo ragdollInfo,
            DamageHandlerBase damageHandlerBase,
            bool isAllowed = true)
        {
            RagdollInfo = ragdollInfo;
            DamageHandlerBase = damageHandlerBase;
            Owner = Player.Get(ragdollInfo.OwnerHub);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Player">Owner</see> of the ragdoll.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets or sets the spawning position of the ragdoll.
        /// </summary>
        public Vector3 Position
        {
            get => RagdollInfo.StartPosition;
            set => RagdollInfo = new RagdollInfo(Owner.ReferenceHub, DamageHandlerBase, value, Rotation);
        }

        /// <summary>
        /// Gets or sets the ragdoll's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => RagdollInfo.StartRotation;
            set => RagdollInfo = new RagdollInfo(Owner.ReferenceHub, DamageHandlerBase, Position, value);
        }

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="RoleType"/>.
        /// </summary>
        public RoleType Role
        {
            get => RagdollInfo.RoleType;
            set => RagdollInfo = new RagdollInfo(Owner.ReferenceHub, DamageHandlerBase, value, Position, Rotation, Nickname, CreationTime);
        }

        /// <summary>
        /// Gets the ragdoll's creation time.
        /// </summary>
        public double CreationTime => RagdollInfo.CreationTime;

        /// <summary>
        /// Gets or sets the ragdoll's nickname.
        /// </summary>
        public string Nickname
        {
            get => RagdollInfo.Nickname;
            set => RagdollInfo = new RagdollInfo(Owner.ReferenceHub, DamageHandlerBase, Role, Position, Rotation, value, CreationTime);
        }

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="global::RagdollInfo"/>.
        /// </summary>
        public RagdollInfo RagdollInfo { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="PlayerStatsSystem.DamageHandlerBase"/>.
        /// </summary>
        public DamageHandlerBase DamageHandlerBase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the ragdoll can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
