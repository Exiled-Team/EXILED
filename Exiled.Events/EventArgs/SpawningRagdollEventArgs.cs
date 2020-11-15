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

    using UnityEngine;

    /// <summary>
    /// Contains all informations before spawning a player ragdoll.
    /// </summary>
    public class SpawningRagdollEventArgs : EventArgs
    {
        private int playerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningRagdollEventArgs"/> class.
        /// </summary>
        /// <param name="killer"><inheritdoc cref="Killer"/></param>
        /// <param name="owner"><inheritdoc cref="Owner"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="rotation"><inheritdoc cref="Rotation"/></param>
        /// <param name="roleType"><inheritdoc cref="RoleType"/></param>
        /// <param name="hinInformations"><inheritdoc cref="HitInformations"/></param>
        /// <param name="isRecallAllowed"><inheritdoc cref="IsRecallAllowed"/></param>
        /// <param name="dissonanceId"><inheritdoc cref="DissonanceId"/></param>
        /// <param name="playerName"><inheritdoc cref="PlayerNickname"/></param>
        /// <param name="playerId"><inheritdoc cref="PlayerId"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningRagdollEventArgs(
            Player killer,
            Player owner,
            Vector3 position,
            Quaternion rotation,
            RoleType roleType,
            PlayerStats.HitInfo hinInformations,
            bool isRecallAllowed,
            string dissonanceId,
            string playerName,
            int playerId,
            bool isAllowed = true)
        {
            Killer = killer;
            Owner = owner;
            Position = position;
            Rotation = rotation;
            RoleType = roleType;
            HitInformations = hinInformations;
            IsRecallAllowed = isRecallAllowed;
            DissonanceId = dissonanceId;
            PlayerNickname = playerName;
            PlayerId = playerId;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who killed the owner of the ragdoll.
        /// </summary>
        public Player Killer { get; }

        /// <summary>
        /// Gets the player, owner of the ragdoll.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets or sets the spawning position of the ragdoll.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the role type of the ragdoll owner.
        /// </summary>
        public RoleType RoleType { get; set; }

        /// <summary>
        /// Gets or sets the hit informations on the ragdoll.
        /// </summary>
        public PlayerStats.HitInfo HitInformations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be revived by SCP-049.
        /// </summary>
        public bool IsRecallAllowed { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll dissonance id.
        /// </summary>
        public string DissonanceId { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll player nickname.
        /// </summary>
        public string PlayerNickname { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll playr id.
        /// </summary>
        public int PlayerId
        {
            get => playerId;
            set
            {
                if (Player.Get(value) == null)
                    return;

                playerId = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
