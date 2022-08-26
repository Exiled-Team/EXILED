// -----------------------------------------------------------------------
// <copyright file="SpawningRagdollEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using PlayerStatsSystem;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before spawning a player ragdoll.
    /// </summary>
    public class SpawningRagdollEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawningRagdollEventArgs" /> class.
        /// </summary>
        /// <param name="ragdollInfo">
        ///     <inheritdoc cref="Info" />
        /// </param>
        /// <param name="damageHandlerBase">
        ///     <inheritdoc cref="DamageHandlerBase" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public SpawningRagdollEventArgs(
            RagdollInfo ragdollInfo,
            DamageHandlerBase damageHandlerBase,
            bool isAllowed = true)
        {
            Info = ragdollInfo;
            DamageHandlerBase = damageHandlerBase;
            Player = Player.Get(ragdollInfo.OwnerHub);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the spawning position of the ragdoll.
        /// </summary>
        public Vector3 Position
        {
            get => Info.StartPosition;
            set => Info = new RagdollInfo(Player.ReferenceHub, DamageHandlerBase, value, Rotation);
        }

        /// <summary>
        ///     Gets or sets the ragdoll's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Info.StartRotation;
            set => Info = new RagdollInfo(Player.ReferenceHub, DamageHandlerBase, Position, value);
        }

        /// <summary>
        ///     Gets or sets the ragdoll's <see cref="RoleType" />.
        /// </summary>
        public RoleType Role
        {
            get => Info.RoleType;
            set => Info = new RagdollInfo(Player.ReferenceHub, DamageHandlerBase, value, Position, Rotation, Nickname, CreationTime);
        }

        /// <summary>
        ///     Gets the ragdoll's creation time.
        /// </summary>
        public double CreationTime
        {
            get => Info.CreationTime;
        }

        /// <summary>
        ///     Gets or sets the ragdoll's nickname.
        /// </summary>
        public string Nickname
        {
            get => Info.Nickname;
            set => Info = new RagdollInfo(Player.ReferenceHub, DamageHandlerBase, Role, Position, Rotation, value, CreationTime);
        }

        /// <summary>
        ///     Gets or sets the ragdoll's <see cref="RagdollInfo" />.
        /// </summary>
        public RagdollInfo Info { get; set; }

        /// <summary>
        ///     Gets or sets the ragdoll's <see cref="PlayerStatsSystem.DamageHandlerBase" />.
        /// </summary>
        public DamageHandlerBase DamageHandlerBase { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the ragdoll can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the <see cref="Player">Owner</see> of the ragdoll.
        /// </summary>
        public Player Player { get; }
    }
}