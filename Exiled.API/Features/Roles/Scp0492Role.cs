// -----------------------------------------------------------------------
// <copyright file="Scp0492Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using UnityEngine;

    /// <summary>
    /// Defines a role that represents SCP-049-2.
    /// </summary>
    public class Scp0492Role : Role
    {
        private Scp049_2PlayerScript script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp0492Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp0492Role(Player player) => Owner = player;

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the actual script of the Scp.
        /// </summary>
        public Scp049_2PlayerScript Scp0492 => script ?? (script = Owner.ReferenceHub.characterClassManager.Scp0492);

        /// <summary>
        /// Gets or sets the SCP-049-2 attack distance.
        /// </summary>
        public float AttackDistance
        {
            get => Scp0492.distance;
            set => Scp0492.distance = value;
        }

        /// <summary>
        /// Gets or sets the SCP-049-2 attack damage.
        /// </summary>
        public float AttackDamage
        {
            get => Scp0492.damage;
            set => Scp0492.damage = value;
        }

        /// <summary>
        /// Gets or sets the amount of time in between SCP-049-2 attacks.
        /// </summary>
        public float AttackCooldown
        {
            get => Scp0492.attackCooldown;
            set => Scp0492.attackCooldown = value;
        }

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp0492;
    }
}
