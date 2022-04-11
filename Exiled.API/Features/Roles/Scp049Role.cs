// -----------------------------------------------------------------------
// <copyright file="Scp049Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayableScps;

    using UnityEngine;

    /// <summary>
    /// Defines a role that represents SCP-049.
    /// </summary>
    public class Scp049Role : Role
    {
        private Scp049 script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp049Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp049Role(Player player) => Owner = player;

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="Scp049"/> player script for this role.
        /// </summary>
        public Scp049 Script => script ?? (script = Owner.CurrentScp as Scp049);

        /// <summary>
        /// Gets a value indicating whether or not SCP-049 is currently recalling a player.
        /// </summary>
        public bool IsRecalling => Script._recallInProgressServer;

        /// <summary>
        /// Gets the player that is currently being revived by SCP-049. Will be <see langword="null"/> if <see cref="IsRecalling"/> is false.
        /// </summary>
        public Player RecallingPlayer
        {
            get
            {
                return !IsRecalling || Script._recallHubServer is null ? null : Player.Get(Script._recallHubServer);
            }
        }

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp049;

        /// <summary>
        /// Gets a boolean indicating whether or not SCP-049 is close enough to a ragdoll to revive it.
        /// <para>
        /// This method only returns whether or not SCP-049 is close enough to the body to revive it; the body may have expired. Make sure to check <see cref="Ragdoll.AllowRecall"/> to ensure the body can be revived.
        /// </para>
        /// </summary>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to revive the body; otherwise, <see langword="false"/>.</returns>
        public bool InRecallRange(Ragdoll ragdoll) => Vector3.Distance(Owner.ReferenceHub.transform.position, ragdoll.Position) <= PlayableScps.Scp049.ReviveDistance * 1.3f;
    }
}
