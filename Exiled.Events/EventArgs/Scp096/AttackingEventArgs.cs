// -----------------------------------------------------------------------
// <copyright file="AttackingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-096 attacks someone.
    /// </summary>
    public class AttackingEventArgs : IScp096Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingEventArgs" /> class.
        /// </summary>
        /// <param name="player"><see cref="Player"/>.</param>
        /// <param name="target"><see cref="Target"/>.</param>
        /// <param name="humanDamage"><see cref="HumanDamage"/>.</param>
        /// <param name="nonTargetDamage"><see cref="NonTargetDamage"/>.</param>
        /// <param name="isAllowed"><see cref="IsAllowed"/>.</param>
        public AttackingEventArgs(Player player, Player target, float humanDamage, float nonTargetDamage, bool isAllowed = true)
        {
            Player = player;
            Scp096 = Player.Role.As<Scp096Role>();
            Target = target;
            HumanDamage = humanDamage;
            NonTargetDamage = nonTargetDamage;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp096Role Scp096 { get; }

        /// <summary>
        /// Gets the player that is going to be damaged by the SCP-096.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the amount of damage of the SCP-096 hit.
        /// </summary>
        public float HumanDamage { get; set; }

        /// <summary>
        /// Gets or sets the amount of damage of the SCP-096 if the target is not a target of the SCP-096.
        /// </summary>
        public float NonTargetDamage { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}