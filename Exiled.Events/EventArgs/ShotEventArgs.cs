// -----------------------------------------------------------------------
// <copyright file="ShotEventArgs.cs" company="Exiled Team">
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
    /// Contains all informations after a player has shot.
    /// </summary>
    public class ShotEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShotEventArgs"/> class.
        /// </summary>
        /// <param name="shooter"><inheritdoc cref="Shooter"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="hitboxType"><inheritdoc cref="HitboxType"/></param>
        /// <param name="distance"><inheritdoc cref="Distance"/></param>
        /// <param name="damage"><inheritdoc cref="Damage"/></param>
        /// <param name="canHurt"><inheritdoc cref="CanHurt"/></param>
        public ShotEventArgs(Player shooter, GameObject target, string hitboxType, float distance, float damage, bool canHurt = true)
        {
            Shooter = shooter;
            Target = target;
            HitboxType = hitboxType;
            Distance = distance;
            Damage = damage;
            CanHurt = canHurt;
        }

        /// <summary>
        /// Gets the player who shot.
        /// </summary>
        public Player Shooter { get; private set; }

        /// <summary>
        /// Gets the target of the shot.
        /// </summary>
        public GameObject Target { get; private set; }

        /// <summary>
        /// Gets the hitbox type of the shot.
        /// </summary>
        public string HitboxType { get; private set; }

        /// <summary>
        /// Gets the shot distance.
        /// </summary>
        public float Distance { get; private set; }

        /// <summary>
        /// Gets or sets the inflicted damage.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the shot can hurt the target or notc.
        /// </summary>
        public bool CanHurt { get; set; }
    }
}