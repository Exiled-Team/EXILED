// -----------------------------------------------------------------------
// <copyright file="ShotEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all information after a player has fired a weapon.
    /// </summary>
    public class ShotEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShotEventArgs"/> class.
        /// </summary>
        /// <param name="shooter"><inheritdoc cref="Shooter"/></param>
        /// <param name="destructible">The <see cref="IDestructible"/> hit.</param>
        /// <param name="hit"><inheritdoc cref="Distance"/></param>
        /// <param name="damage"><inheritdoc cref="Damage"/></param>
        public ShotEventArgs(Player shooter, RaycastHit hit, IDestructible destructible, float damage) {
            Shooter = shooter;
            Damage = damage;
            Distance = hit.distance;

            if (destructible is HitboxIdentity identity) {
                Hitbox = identity;
                Target = Player.Get(Hitbox.TargetHub);
            }
        }

        /// <summary>
        /// Gets the player who shot.
        /// </summary>
        public Player Shooter { get; }

        /// <summary>
        /// Gets the target of the shot. Can be <see langword="null"/>!.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the hitbox type of the shot. Can be <see langword="null"/>!.
        /// </summary>
        public HitboxIdentity Hitbox { get; }

        /// <summary>
        /// Gets the shot distance.
        /// </summary>
        public float Distance { get; }

        /// <summary>
        /// Gets or sets the inflicted damage.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the shot can hurt the target.
        /// </summary>
        public bool CanHurt { get; set; } = true;
    }
}
