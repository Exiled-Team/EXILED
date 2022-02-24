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

        /// <summary>
        /// Gets the actual script of the Scp.
        /// </summary>
        public Scp049 Scp049 => script ?? (script = Owner.ReferenceHub.scpsController.CurrentScp as Scp049);

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-049 is currently recalling a player.
        /// </summary>
        public bool IsRecalling => Scp049._recallInProgressServer;

        /// <summary>
        /// Gets the player that is currently being revived by SCP-049. Will be <see langword="null"/> if <see cref="IsRecalling"/> is false.
        /// </summary>
        public Player RecallingPlayer
        {
            get
            {
                if (!IsRecalling || Scp049._recallHubServer == null)
                    return null;

                return Player.Get(Scp049._recallHubServer);
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
        public bool InRecallRange(Ragdoll ragdoll) => Vector3.Distance(Owner.ReferenceHub.transform.position, ragdoll.Position) <= Scp049.ReviveDistance * 1.3f;
    }
}
