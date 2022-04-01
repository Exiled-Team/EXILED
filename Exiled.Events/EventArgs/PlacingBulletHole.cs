// -----------------------------------------------------------------------
// <copyright file="PlacingBulletHole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
#pragma warning disable SA1401
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using InventorySystem.Items.Firearms.Modules;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before placing a decal.
    /// </summary>
    public class PlacingBulletHole : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingBulletHole"/> class.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Owner"/></param>
        /// <param name="hit"><inheritdoc cref="RaycastHit"/></param>
        public PlacingBulletHole(Player owner, RaycastHit hit) {
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
