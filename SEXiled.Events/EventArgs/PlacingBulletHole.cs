// -----------------------------------------------------------------------
// <copyright file="PlacingBulletHole.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
#pragma warning disable SA1401
    using System;

    using SEXiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before placing a bullet hole decal.
    /// </summary>
    public class PlacingBulletHole : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingBulletHole"/> class.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Owner"/></param>
        /// <param name="hit"><inheritdoc cref="RaycastHit"/></param>
        public PlacingBulletHole(Player owner, RaycastHit hit)
        {
            Owner = owner;
            Position = hit.point;
            Rotation = Quaternion.LookRotation(hit.normal);
        }

        /// <summary>
        /// Gets the decal owner.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets or sets the decal position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the decal rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the decal can be placed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
