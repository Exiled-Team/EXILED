// -----------------------------------------------------------------------
// <copyright file="ShotEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;

    using UnityEngine;

    /// <summary>
    ///     Contains all information after a player has fired a weapon.
    /// </summary>
    public class ShotEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ShotEventArgs" /> class.
        /// </summary>
        /// <param name="shooter">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        ///     <inheritdoc cref="Firearm"/>
        /// </param>
        /// <param name="destructible">The <see cref="IDestructible" /> hit.</param>
        /// <param name="hit">
        ///     <inheritdoc cref="Distance" />
        /// </param>
        /// <param name="damage">
        ///     <inheritdoc cref="Damage" />
        /// </param>
        public ShotEventArgs(Player shooter, Firearm firearm, RaycastHit hit, IDestructible destructible, float damage)
        {
            Player = shooter;
            Firearm = firearm;
            Damage = damage;
            Distance = hit.distance;
            Position = hit.point;
            RaycastHit = hit;

            if (destructible is HitboxIdentity identity)
            {
                Hitbox = identity;
                Target = Player.Get(Hitbox.TargetHub);
            }
        }

        /// <summary>
        ///     Gets the player who shot.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the firearm used to shoot.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        ///     Gets the hitbox type of the shot. Can be <see langword="null" />!.
        /// </summary>
        public HitboxIdentity Hitbox { get; }

        /// <summary>
        ///     Gets or sets the inflicted damage.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        ///     Gets the shot distance.
        /// </summary>
        public float Distance { get; }

        /// <summary>
        ///     Gets the shot position.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        ///     Gets the raycast result.
        /// </summary>
        public RaycastHit RaycastHit { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the shot can hurt the target.
        /// </summary>
        public bool CanHurt { get; set; } = true;

        /// <summary>
        ///     Gets the target of the shot. Can be <see langword="null" />!.
        /// </summary>
        public Player Target { get; }
    }
}
