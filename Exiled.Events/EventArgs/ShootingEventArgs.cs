// -----------------------------------------------------------------------
// <copyright file="ShootingEventArgs.cs" company="Exiled Team">
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
    /// Contains all informations before shooting with a weapon.
    /// </summary>
    public class ShootingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingEventArgs"/> class.
        /// </summary>
        /// <param name="shooter"><inheritdoc cref="Shooter"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ShootingEventArgs(Player shooter, GameObject target, Vector3 position, bool isAllowed = true)
        {
            Shooter = shooter;
            Target = target;
            Position = position;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's shooting.
        /// </summary>
        public Player Shooter { get; private set; }

        /// <summary>
        /// Gets the target the player's shooting at.
        /// </summary>
        public GameObject Target { get; private set; }

        /// <summary>
        /// Gets or sets the position of the shoot.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}